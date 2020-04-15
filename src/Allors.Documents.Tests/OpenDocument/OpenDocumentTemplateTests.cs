// <copyright file="OpenDocumentTemplateTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allos.Document.OpenDocument.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Allors.Document.OpenDocument;

    using Xunit;

    public partial class OpenDocumentTemplateTests
    {
        [Fact]
        public void Render()
        {
            var model = new Model
            {
                Person = new ModelPerson { FirstName = "Jane" },
                People = new[]
                         {
                             new ModelPerson { FirstName = "John" },
                             new ModelPerson { FirstName = "Jenny" },
                         },
                Images = new[]
                             {
                                 "number1",
                                 "number2",
                                 "number3",
                             },
            };

            var document = this.GetResource("EmbeddedTemplate.odt");
            var template = new OpenDocumentTemplate<Model>(document);

            var images = new Dictionary<string, byte[]>
                             {
                                 { "logo", this.GetResource("logo.png") },
                                 { "logo2", this.GetResource("logo.png") },
                                 { "number1", this.GetResource("1.png") },
                                 { "number2", this.GetResource("2.png") },
                                 { "number3", this.GetResource("3.png") },
                             };

            var result = template.Render(model, images);

            //var desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //File.WriteAllBytes(Path.Combine(desktopDir, "generated.odt"), result);

            // TODO: Check in generated document if it worked!
        }

        [Fact]
        public void Rerender()
        {
            var model = new Model
            {
                Person = new ModelPerson { FirstName = "Jane" },
                People = new[]
                                             {
                                                 new ModelPerson { FirstName = "John" },
                                                 new ModelPerson { FirstName = "Jenny" },
                                             },
                Images = new[]
                             {
                                 "number1",
                                 "number2",
                                 "number3",
                             },
            };

            var document = this.GetResource("EmbeddedTemplate.odt");
            var template = new OpenDocumentTemplate<Model>(document);
            var images = new Dictionary<string, byte[]>
                             {
                                 { "logo", this.GetResource("logo.png") },
                                 { "logo2", this.GetResource("logo.png") },
                                 { "number1", this.GetResource("1.png") },
                                 { "number2", this.GetResource("2.png") },
                                 { "number3", this.GetResource("3.png") },
                             };

            // warmup
            for(var i=0; i<10; i++)
            {
                template.Render(model, images);
            }

            // run 1
            var watch = System.Diagnostics.Stopwatch.StartNew();

            template.Render(model, images);

            watch.Stop();
            var run1 = watch.ElapsedMilliseconds;

            // run 2
            watch = System.Diagnostics.Stopwatch.StartNew();

            template.Render(model, images);

            watch.Stop();
            var run2 = watch.ElapsedMilliseconds;

            Assert.InRange(run2, 0, run1 * 2);
        }

        private byte[] GetResource(string name)
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;

            var resourceName = assembly.GetManifestResourceNames().First(v => v.Contains(name));
            var resource = assembly.GetManifestResourceStream(resourceName);

            using (var output = new MemoryStream())
            {
                resource?.CopyTo(output);
                return output.ToArray();
            }
        }
    }
}
