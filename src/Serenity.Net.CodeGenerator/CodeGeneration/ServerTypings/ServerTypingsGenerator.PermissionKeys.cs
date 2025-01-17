﻿using Mono.Cecil;
using Serenity.Reflection;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : CecilImportGenerator
    {
        protected void GeneratePermissionKeys(TypeDefinition type)
        {
            GeneratePermissionKeysFor(type, declare: true);
        }

        protected void GeneratePermissionKeysFor(TypeDefinition type, bool declare)
        {
            cw.Indented(declare ? "export namespace ": "namespace ");
            sb.Append(type.Name);
            cw.InBrace(delegate
            {
                foreach (var fi in type.Fields.Where(x =>
                    x.IsPublic && x.IsStatic && x.HasConstant && x.Constant is string &&
                    x.DeclaringType.FullName == type.FullName &&
                    x.FieldType.FullName == "System.String"))
                {
                    cw.Indented("export const ");
                    sb.Append(fi.Name);
                    sb.Append(" = ");
                    sb.Append(System.Text.Json.JsonSerializer.Serialize(fi.Constant as string));
                    sb.AppendLine(";");
                }

                if (!type.HasNestedTypes)
                    return;

                foreach (var nested in type.NestedTypes)
                {
                    if (CecilUtils.GetAttr(type, "Serenity.ComponentModel", "ScriptSkipAttribute") != null)
                        continue;

                    sb.AppendLine();
                    GeneratePermissionKeysFor(nested, declare: false);
                }
            });
        }
    }
}