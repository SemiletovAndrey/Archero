using Template.Features.Popups;

namespace Contexts.Persistance.Auth
{
    public class TestPopupPayload : IPopupPayload
    {
        public TestPopupPayload(int value)
        {
            Value = value;
        }

        public int Value {  get; private set; }
    }
}