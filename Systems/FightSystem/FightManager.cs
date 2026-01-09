using Godot;
using System;

public partial class FightManager : Control
{

    [Export] public ProgressBar playerOneBeatMeater;
    [Export] public ProgressBar playerTwoBeatMeater;

    [Export] public RichTextLabel playerOneMultiplierDisplay;
    [Export] public Control playerOneMultiplierDisplayHolder;
    [Export] public AnimationPlayer playerOneMultiplierDisplayAnimator;




    [Export] public AnimationPlayer countdownAnimator;
    [Export] public Button fightButton;
    
    [Export] public AnimationPlayer playerDamageAniamtor;
    [Export] public AnimationPlayer oppDamageAniamtor;



    public Player playerOne;
    public BaseNpc playerTwo;

    public Variant winner;

    private bool inputLock = true;

    private float playerOneHealth;
    private float playerTwoHealth;

    Tween tween;


    public override void _Ready()
    {
        countdownAnimator.Play("countDown");
    }



    public void SetupFight()
    {
        playerOne.BeatInputed += _OnPlayerOneBeat;
        playerTwo.BeatInputed += _OnPlayerTwoBeat;

        playerTwo.fightCountdownTimer.Timeout += StartFightSquence;

        playerOneHealth = playerOne.beaterDataComponent.beaterData.health;
        playerTwoHealth = playerTwo.beaterDataComponent.beaterData.health;

        playerOneBeatMeater.MaxValue = playerOneHealth;
        playerTwoBeatMeater.MaxValue = playerTwoHealth;
        
        playerOneBeatMeater.Value = playerOneHealth;
        playerTwoBeatMeater.Value = playerTwoHealth;

    }

    public void StartFightSquence()
    {
        inputLock = false;
        
    }

    public void _OnPlayerOneBeat()
    {
        if (inputLock) return;

        float randomDamageMultiplier = 1.0f;
        if (GD.Randf() < 0.1f)
        {
            float randVal = GD.Randf();
            randomDamageMultiplier = 1.0f + randVal;
            ShakeMultiplierDisplay(playerOneMultiplierDisplayHolder, playerOneMultiplierDisplay, randomDamageMultiplier);
        }

        playerTwoHealth -= playerOne.beaterDataComponent.beaterData.damage * randomDamageMultiplier + playerOne.beaterDataComponent.beaterData.damageMultiplier * 1.0f;

        playerTwo.PlayGoonDamageSound();
        oppDamageAniamtor.Play("oppDamage");

        if (playerTwoHealth <= 0.0f)
        {
            winner = playerOne;
            EndFightSquence();
        }

        playerTwoBeatMeater.Value = playerTwoHealth;

        ResetTween();
        tween.SetParallel(true);
    
        // Scale up and back
        tween.TweenProperty(fightButton, "scale", Vector2.One * 1.2f, 0.05).SetTrans(Tween.TransitionType.Expo);
        tween.TweenProperty(fightButton, "scale", Vector2.One, 0.05).SetDelay(0.05);
        
        // Shake rotation
        float randomRotation = Mathf.DegToRad(GD.Randf() * 20 - 10 * Mathf.Clamp((1f-playerTwoHealth),1,9));
        tween.TweenProperty(fightButton, "rotation", randomRotation, 0.05).SetTrans(Tween.TransitionType.Expo);
        tween.TweenProperty(fightButton, "rotation", 0f, 0.05).SetDelay(0.05);
    }


    public void _OnPlayerTwoBeat()
    {
        if (inputLock) return;

        playerOneHealth -= playerTwo.beaterDataComponent.beaterData.damage + playerTwo.beaterDataComponent.beaterData.damageMultiplier * 1.0f;
        playerDamageAniamtor.Play("damagePlayer");

        if (playerOneHealth <= 0.0f)
        {
            winner = playerTwo;
            EndFightSquence();
        }

        playerOneBeatMeater.Value = playerOneHealth;
    }

    public async void EndFightSquence()
    {
        inputLock = true;
        playerOne.BeatInputed -= _OnPlayerOneBeat;
        playerTwo.BeatInputed -= _OnPlayerTwoBeat;

        playerTwo.fightCountdownTimer.Timeout -= StartFightSquence;
        if (winner.AsGodotObject() == playerTwo)
        {
            playerTwo.PlayWinAnim();
        }
        else
        {
            playerTwo.PlayLoseAnim();
        }

        await ToSignal(playerTwo.fightAnimator, AnimationPlayer.SignalName.AnimationFinished);

        SignalBus.Instance.EmitSignal(SignalBus.SignalName.FightEnded, playerTwo, winner);
    }

    private void ResetTween()
    {
        if (tween != null) tween.Kill();   
        tween = CreateTween();
    }

    private void ShakeMultiplierDisplay(Control labelHolder, RichTextLabel richTextLabel, float displayValue)
    {
        richTextLabel.Text = "[rainbow]" + Convert.ToString(MathF.Round(displayValue, 1)) + "x";

        playerOneMultiplierDisplayAnimator.Play("dispMult");
    }
}
