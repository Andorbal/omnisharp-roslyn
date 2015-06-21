using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using OmniSharp.Models;
using Xunit;

namespace OmniSharp.Tests
{
    public class GoToDefinitionFacts
    {
        [Fact]
        public async Task ReturnsLocationSourceDefinition()
        {
            var source1 = @"using System;

class Foo {
}";
            var source2 = @"class Bar {
    private Foo foo;
}";

            var workspace = TestHelpers.CreateSimpleWorkspace(new Dictionary<string, string> {
                { "foo.cs", source1 }, { "bar.cs", source2}
            });
            var controller = new OmnisharpController(workspace, null);
            var response = await controller.GotoDefinition(new Request
            {
                FileName = "bar.cs",
                Line = 2,
                Column = 14
            }) as ObjectResult;

            var definitionResponse = response.Value as GotoDefinitionResponse;

            Assert.Equal("foo.cs", definitionResponse.FileName);
            Assert.Equal(3, definitionResponse.Line);
            Assert.Equal(7, definitionResponse.Column);
        }

        [Fact]
        public async Task ReturnsEmptyResultWhenDefinitionIsNotFound()
        {
            var source1 = @"using System;

class Foo {
}";
            var source2 = @"class Bar {
    private Baz foo;
}";

            var workspace = TestHelpers.CreateSimpleWorkspace(new Dictionary<string, string> {
                { "foo.cs", source1 }, { "bar.cs", source2}
            });
            var controller = new OmnisharpController(workspace, null);
            var response = await controller.GotoDefinition(new Request
            {
                FileName = "bar.cs",
                Line = 2,
                Column = 14
            }) as ObjectResult;

            var definitionResponse = response.Value as GotoDefinitionResponse;

            Assert.Null(definitionResponse.FileName);
            Assert.Equal(0, definitionResponse.Line);
            Assert.Equal(0, definitionResponse.Column);
        }
    }
}
