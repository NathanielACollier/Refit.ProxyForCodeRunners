using Microsoft.VisualStudio.TestTools.UnitTesting;
using Refit;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class AppleComputerWebAPITests
    {

        public interface AppleEntityLookupApi
        {
            [Get("/lookup?id={id}&entity=podcast")]
            Task<string> GetPodCast(string id);
        }

        static AppleEntityLookupApi api;

        [ClassInitialize()]
        public static void ClassInit(TestContext context) {
            api = Refit.ProxyForCodeRunners.ProxyRestService.For<AppleEntityLookupApi>("https://itunes.apple.com");
        }


        [TestMethod]
        public async Task TestGetPodcastByID()
        {
            var entry = await api.GetPodCast("354869137");

            Assert.IsFalse(string.IsNullOrEmpty(entry), "Entry was empty");

        }
    }
}
