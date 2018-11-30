using MatchingGame.Blazor.App;
using MatchingGame.Logic;
using Microsoft.AspNetCore.Blazor.Components;
using System.Threading.Tasks;

namespace MatchingGame.Blazor
{
    public class MatchingCardGameBase : BlazorComponent
    {
        protected Game game;
        protected int bestScore;
        protected string message;
        bool isBusy;

        protected override void OnInit()
        {
            StartNewGame();
        }

        protected void StartNewGame()
        {
            game = Game.Create();
            message = null;
        }

        protected async Task OnCardSelected(Card card)
        {
            if (isBusy || game.IsOpen(card.Column, card.Row))
            {
                return;
            }

            game.OpenCard(card.Column, card.Row);
            StateHasChanged();

            if (game.RemainingCardsInTurn == 0)
            {
                CheckForWinner();

                if (!game.IsMatch())
                {
                    isBusy = true;
                    await Task.Delay(750);
                    isBusy = false;
                }

                game.CompleteTurn();
                StateHasChanged();
            }
        }

        private void CheckForWinner()
        {
            if (!game.IsComplete())
                return;

            UpdateBestScore();

            var currentScore = game.Turns;
            if (bestScore == 0)
                message = $"It took you {currentScore} turns to complete the game. Keep it up!";
            else if (bestScore < currentScore)
                message = $"It took you {currentScore} turns to complete the game! The best score is {bestScore} turns.";
            else
                message = $"You set a new best with only {currentScore} turns!";
        }

        private void UpdateBestScore()
        {
            if (bestScore == 0 || game.Turns < bestScore)
                bestScore = game.Turns;
        }

    }
}
