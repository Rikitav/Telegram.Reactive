using Telegram.Reactive.Core.Components.StateKeeping;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;
using Telegram.Reactive.StateKeeping;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers
{
    public enum UserRegisterState
    {
        WaitingName,
        WaitingAge,
        WaitingEmail
    }

    [MessageHandler]
    public class UserRegisterHandler : BranchingCommandHandler
    {
        [CommandHandler(Priority = 10), CommandAllias("register"), EnumState<UserRegisterState>(SpecialState.NoState)]
        public async Task Register()
        {
            await Reply("Форма регистрации открыта", cancellationToken: Cancellation);
            await Reply("Пожалуйста отправьте свое имя", cancellationToken: Cancellation);

            Container.SetEnumState(UserRegisterState.WaitingName);

            Console.WriteLine("register");
        }

        [MessageRegex(@"\S+"), EnumState<UserRegisterState>(UserRegisterState.WaitingName)]
        public async Task WaitName()
        {
            await Reply("Пожалуйста отправьте свой возраст", cancellationToken: Cancellation);
            Container.SetEnumState(UserRegisterState.WaitingAge);
        }

        [MessageRegex(@"\d+"), EnumState<UserRegisterState>(UserRegisterState.WaitingAge)]
        public async Task WaitingAge()
        {
            await Reply("Пожалуйста отправьте свою электронную почту", cancellationToken: Cancellation);
            Container.SetEnumState(UserRegisterState.WaitingEmail);
        }

        [MessageRegex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"), EnumState<UserRegisterState>(UserRegisterState.WaitingEmail)]
        public async Task WaitingEmail()
        {
            await Reply("Спасибо за прохождение регистрации. Форма закрыта", cancellationToken: Cancellation);
            Container.DeleteEnumState<UserRegisterState>();
        }

        [CommandHandler, CommandAllias("cancell"), EnumState<UserRegisterState>(SpecialState.AnyState)]
        public async Task Cancell()
        {
            await Reply("Форма регистрации закрыта", cancellationToken: Cancellation);
            Container.DeleteEnumState<UserRegisterState>();
        }

        [MessageHandler(Priority = -10), EnumState<UserRegisterState>(SpecialState.AnyState), IsReleaseEnvironment]
        public async Task AnyState()
        {
            switch (Container.EnumStateKeeper<UserRegisterState>().GetState(HandlingUpdate))
            {
                case UserRegisterState.WaitingName:
                    await Reply("Пожалуйста отправьте свое имя", cancellationToken: Cancellation);
                    break;

                case UserRegisterState.WaitingAge:
                    await Reply("Пожалуйста отправьте свой возраст", cancellationToken: Cancellation);
                    break;

                case UserRegisterState.WaitingEmail:
                    await Reply("Пожалуйста отправьте свою электронную почту", cancellationToken: Cancellation);
                    break;
            }
        }
    }
}
