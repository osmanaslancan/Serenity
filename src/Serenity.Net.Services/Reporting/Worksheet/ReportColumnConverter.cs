﻿using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Reporting
{
    public static class ReportColumnConverter
    {
        private static ReportColumn FromMember(MemberInfo member, Type dataType,
            Field baseField, ITextLocalizer localizer)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            var result = new ReportColumn
            {
                Name = member.Name
            };
            var displayAttr = member.GetCustomAttribute<DisplayNameAttribute>();
            if (displayAttr != null)
                result.Title = displayAttr.DisplayName;

            var sizeAttr = member.GetCustomAttribute<SizeAttribute>();
            if (sizeAttr != null && sizeAttr.Value != 0)
                result.Width = sizeAttr.Value;

            var formatAttr = member.GetCustomAttribute<DisplayFormatAttribute>();
            if (formatAttr != null)
                result.Format = formatAttr.Value;
            else
            {
                var dtf = baseField as DateTimeField;
                if (dtf is object && !dtf.DateOnly)
                {
                    result.Format = "dd/MM/yyyy HH:mm";
                }
                else if (dtf is object ||
                    dataType == typeof(DateTime) ||
                    dataType == typeof(DateTime?))
                {
                    result.Format = "dd/MM/yyyy";
                }
            }

            if (baseField is object)
            {
                if (result.Title == null)
                    result.Title = baseField.GetTitle(localizer);

                if (result.Width == null && baseField is StringField && baseField.Size != 0)
                    result.Width = baseField.Size;
            }

            result.DataType = dataType;

            return result;
        }

        public static ReportColumn FromFieldInfo(FieldInfo field, ITextLocalizer localizer, Field baseField = null)
        {
            return FromMember(field, field.FieldType, baseField, localizer);
        }

        public static ReportColumn FromPropertyInfo(PropertyInfo property, ITextLocalizer localizer, Field baseField = null)
        {
            return FromMember(property, property.PropertyType, baseField, localizer);
        }

        public static List<ReportColumn> ObjectTypeToList(Type objectType, 
            IServiceProvider serviceProvider, ITextLocalizer localizer)
        {
            var list = new List<ReportColumn>();

            IRow basedOnRow = null;
            var basedOnRowAttr = objectType.GetCustomAttribute<BasedOnRowAttribute>();
            if (basedOnRowAttr != null)
                basedOnRow = Activator.CreateInstance(basedOnRowAttr.RowType) as IRow;

            foreach (MemberInfo member in objectType.GetMembers(BindingFlags.Instance | BindingFlags.Public))
            {
                var fieldInfo = member as FieldInfo;
                var propertyInfo = member as PropertyInfo;
                if (fieldInfo == null &&
                    propertyInfo == null)
                    continue;

                if (member.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                Field baseField;
                if (basedOnRow != null)
                {
                    var name = ((MemberInfo)fieldInfo ?? propertyInfo).Name;
                    baseField = basedOnRow.FindFieldByPropertyName(name) ?? basedOnRow.FindField(name);
                }
                else
                {
                    baseField = null;
                }

                ReportColumn column;
                if (fieldInfo != null)
                    column = FromFieldInfo(fieldInfo, localizer, baseField);
                else
                    column = FromPropertyInfo(propertyInfo, localizer, baseField);

                var cellDecorator = member.GetCustomAttribute<CellDecoratorAttribute>();
                if (cellDecorator != null)
                {
                    var decorator = (ICellDecorator)ActivatorUtilities.CreateInstance(
                        serviceProvider, cellDecorator.DecoratorType);
                    column.Decorator = decorator;
                }

                list.Add(column);
            }

            return list;
        }

        public static ReportColumn FromField(Field field, ITextLocalizer localizer)
        {
            var column = new ReportColumn
            {
                Name = field.Name,
                Title = field.GetTitle(localizer)
            };

            if (field is StringField)
                if (field.Size != 0)
                    column.Width = field.Size;

            return column;
        }

        public static List<ReportColumn> EntityTypeToList(IRow instance, ITextLocalizer localizer)
        {
            var list = new List<ReportColumn>();

            foreach (var field in instance.GetFields())
            {
                list.Add(FromField(field, localizer));
            }

            return list;
        }
    }
}