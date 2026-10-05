using Template.Features.Log;
using UnityEngine;
using Zenject;

namespace Template.Features.Cheat
{
    public class SampleCheatHandler : BaseCheatHandler
    {
        private IDebugLogger _logger;

        [Inject]
        private void Construct(IDebugLogger logger)
        {
            _logger = logger;
        }

        [CheatCommand("help", "General")]
        private void Help()
        {
            _logger.LogMessage("Sample cheats: help, echo <text>, add_coins <amount>, set_speed <value>, god_mode <enabled>, set_time_scale <value>");
        }

        [CheatCommand("echo", "General")]
        private void Echo([CheatArgument("text")] string text = "hello")
        {
            _logger.LogMessage($"Echo: {text}");
        }

        [CheatCommand("add_coins", "Economy")]
        private void AddCoins([CheatArgument("amount")] int amount = 100)
        {
            _logger.LogMessage($"Added coins: {amount}");
        }

        [CheatCommand("set_speed", "Gameplay")]
        private void SetSpeed([CheatArgument("value")] float value = 1.5f)
        {
            _logger.LogMessage($"Player speed set to {value:0.###}");
        }

        [CheatCommand("god_mode", "Gameplay")]
        private void SetGodMode([CheatArgument("enabled")] bool enabled = true)
        {
            _logger.LogWarning($"God mode: {(enabled ? "ON" : "OFF")}");
        }

        [CheatCommand("set_time_scale", "System")]
        private void SetTimeScale([CheatArgument("value")] float value = 1f)
        {
            Time.timeScale = Mathf.Max(0f, value);
            _logger.LogWarning($"Time scale set to {Time.timeScale:0.###}");
        }
    }
}
