﻿using Serenity.ComponentModel;
using Serenity.Data;
using Xunit;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Reads_LocalTextPrefix_SetInCode()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithLocalTextPrefixSetInCodeRow.ts", result.Keys);
            var code = result["SomeModule.RowWithLocalTextPrefixSetInCodeRow.ts"];
            Assert.Contains("localTextPrefix = 'Set.InCode'", code);
        }

        [Fact]
        public void LocalTextPrefix_SetInCode_Overrides_Attribute()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithLocalTextPrefixSetInCodeAndAttributeRow.ts", result.Keys);
            var code = result["SomeModule.RowWithLocalTextPrefixSetInCodeAndAttributeRow.ts"];
            Assert.Contains("localTextPrefix = 'This.ShouldOverride'", code);
        }

        [Fact]
        public void Reads_LocalTextPrefix_SetWithAttribute()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithLocalTextPrefixAttributeRow.ts", result.Keys);
            var code = result["SomeModule.RowWithLocalTextPrefixAttributeRow.ts"];
            Assert.Contains("localTextPrefix = 'Attribute.Prefix'", code);
        }

        [Fact]
        public void Respects_Module_Attribute_For_Auto_LocalTextPrefix()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithModuleAndNoLocalTextPrefixRow.ts", result.Keys);
            var code = result["SomeModule.RowWithModuleAndNoLocalTextPrefixRow.ts"];
            Assert.Contains("localTextPrefix = 'ADifferentModule.RowWithModuleAndNoLocalTextPrefix'", code);
        }

        [Fact]
        public void Uses_Namespace_And_TypeName_When_No_LocalTextPrefix_Or_Module()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.NoModuleNoLocalTextPrefixRow.ts", result.Keys);
            var code = result["SomeModule.NoModuleNoLocalTextPrefixRow.ts"];
            Assert.Contains("localTextPrefix = 'SomeModule.NoModuleNoLocalTextPrefix'", code);
        }
    }
}

namespace ServerTypingsTest.SomeModule.Entities
{
    public class RowWithLocalTextPrefixSetInCodeRow : Row<RowWithLocalTextPrefixSetInCodeRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public RowFields()
            {
                LocalTextPrefix = "Set.InCode";
            }
        }
    }

    [LocalTextPrefix("This.ShouldBeOverridden")]
    public class RowWithLocalTextPrefixSetInCodeAndAttributeRow : Row<RowWithLocalTextPrefixSetInCodeAndAttributeRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public RowFields()
            {
                LocalTextPrefix = "This.ShouldOverride";
            }
        }
    }

    [LocalTextPrefix("Attribute.Prefix")]
    public class RowWithLocalTextPrefixAttributeRow : Row<RowWithLocalTextPrefixAttributeRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }
    
    [Module("ADifferentModule")]
    public class RowWithModuleAndNoLocalTextPrefixRow : Row<RowWithModuleAndNoLocalTextPrefixRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }

    public class NoModuleNoLocalTextPrefixRow : Row<NoModuleNoLocalTextPrefixRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }
}