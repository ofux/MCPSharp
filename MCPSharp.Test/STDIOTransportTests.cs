﻿using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2.HPack;

namespace MCPSharp.Test
{
    [TestClass]
    public sealed class STDIOTransportTests
    {
        private readonly static MCPClient client = new("Test Client", "1.0.0", "MCPSharp.Example.exe");

        [ClassCleanup]
        public static void ClassCleanup() { client?.Dispose(); }

        [TestCategory("Tools")]
        [TestMethod("Tools/List")]
        public async Task Test_ListTools()
        {
            var tools = await client.GetToolsAsync();
            Assert.IsNotNull(tools);
            Assert.IsTrue(tools.Count > 0);
            tools.ForEach(tool =>
            {
                Assert.IsFalse(string.IsNullOrEmpty(tool.Name));
                Assert.IsFalse(string.IsNullOrEmpty(tool.Description));

                Console.WriteLine(tool.Name);
            });
        }

        [TestCategory("Tools")]
        [TestMethod("Tools/Call")]
        public async Task TestCallTool()
        {
            var result = await client.CallToolAsync("Hello");
            string response = result.Content[0].Text;
            Assert.AreEqual("hello, claude.", response);
        }


        [TestCategory("Tools")]
        [TestMethod("Tools/Call with semantic kernel function")]
        public async Task TestCallTool_semantic()
        {
            var result = await client.CallToolAsync("SemanticTest");
            string response = result.Content[0].Text;
            Assert.AreEqual("success", response);
        }

        [TestCategory("Tools")]
        [TestMethod("Tools/Call with Parameters")]
        public async Task TestCallToolWithParameters()
        {
            var result = await client.CallToolAsync("Echo", new Dictionary<string, object> { { "input", "this is a test of the echo function" } });
            string response = result.Content[0].Text;
            Assert.AreEqual("this is a test of the echo function", response);
        }

        [TestCategory("Tools")]
        [TestMethod("Exception Handling")]
        public async Task TestException()
        {
            var result = await client.CallToolAsync("throw_exception");
            string response = result.Content[0].Text;
            Assert.AreEqual("This is an exception", response);
        }

        [TestCategory("Tools")]
        [TestMethod("Tools/Call with Invalid Tool")]
        public async Task TestCallInvalidTool()
        {
            Assert.IsTrue((await client.CallToolAsync("NotARealTool")).IsError);
        }

        [TestCategory("Tools")]
        [TestMethod("Tools/Call with Invalid Parameters")]
        public async Task TestCallToolWithInvalidParameters()
        {
            var result = await client.CallToolAsync("Echo", new Dictionary<string, object> { { "invalid_param", "test" } });
            Assert.IsTrue(result.IsError);
        }

        [TestMethod("Tools/Call with dll tool")] 
        public async Task TestCallExternalTool()
        {
            var result = await client.CallToolAsync("dll-tool");
            string response = result.Content[0].Text;
            Assert.AreEqual("success", response);
        }


        [TestMethod("List Prompts")]
        public async Task TestListPrompts()
        {
            var result = await client.GetPromptListAsync();
            Assert.IsFalse(result.Prompts.Count != 0);
        }

        [TestMethod("Test Ping")]
        public async Task TestPing()
        {
            await client.PingAsync();
        }

        [TestMethod("Resources/List")]
        public async Task TestResources()
        {
            var result = await client.GetResourcesAsync(); 
            Assert.IsTrue(result.Resources.Count != 0);
            result.Resources.ForEach(result =>
            {
                Console.WriteLine(result.Name);
            });
        }
    }
}
