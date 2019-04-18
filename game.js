var game;

var gameOptions = {
    bigCircleRadius: 500,
    playerRadius: 50,
    playerSpeed: 1,
    worldGravity: 0.8,
    jumpForce: [12, 8]
}

window.onload = function() {
    var gameConfig = {
        thpe: Phaser.CANVAS,
        width: 800,
        hight: 800,
        scene: [playGame]
    }

    game = new Phaser.Game(gameConfig);
    window.focus();
    resize();
    window.addEventListener("resize", resize, false);
}


class playGame extends Phaser.Scene {
    constructor() {
        super("PlayGame");
    }

    preload() {
        this.load.image("bigcircle", "assets/bigcircle.png");
        this.load.image("player", "assets/player.png");
    }

    create() {
        this.bigCircle = this.add.sprite(game.config.width / 2, game.config.height / 2, "bigcircle");
        this.bigCircle.displayWidth = gameOptions.bigCircleRadius;
        this.bigCircle.displayHeight = gameOptions.bigCircleRadius;

        this.player = this.add.sprite(game.config.width / 2, 
            (game.config.height - gameOptions.bigCircleRadius - gameOptions.playerRadius) / 2, "player");
        this.player.displayWidth = gameOptions.playerRadius;
        this.player.displayHeight = gameOptions.playerRadius;
        this.player.currentAngle = -90;
        this.player.jumpOffset = 0;
        this.player.jumps = 0;
        this.player.jumpForce = 0;
            
        this.input.on("pointerdown", function(e) {
            if (this.player.jumps < 2) {
                this.player.jumps++;
                this.player.jumpForce = gameOptions.jumpForce[this.player.jumps - 1];
            }
        }, this);
    }

    update() {
        if (this.player.jumps > 0) {
            this.player.jumpOffset += this.player.jumpForce;
            this.player.jumpForce -= gameOptions.worldGravity;

            if (this.player.jumpOffset < 0) {
                this.player.jumpOffset = 0;
                this.player.jumps = 0;
                this.player.jumpForce = 0;
            }
        }

        this.player.currentAngle = Phaser.Math.Angle.WrapDegrees(this.player.currentAngle + gameOptions.playerSpeed);
        var radians = Phaser.Math.DegToRad(this.player.currentAngle);
        var distanceFromCenter = (gameOptions.bigCircleRadius + gameOptions.playerRadius) / 2 + this.player.jumpOffset;
        this.player.x = this.bigCircle.x + distanceFromCenter * Math.cos(radians);
        this.player.y = this.bigCircle.y + distanceFromCenter * Math.sin(radians);
        var revolutions = gameOptions.bigCircleRadius / gameOptions.playerRadius + 1;
        this.player.angle = this.player.currentAngle * revolutions;
    }
}


// Resize the game window to fit various displays
function resize() {
    var canvas = document.querySelector("canvas");
    var windowWidth = window.innerWidth;
    var windowHeight = window.innerHeight;
    var windowRatio = windowWidth / windowHeight;
    var gameRatio = game.config.width / game.config.height;

    if (windowRatio < gameRatio) {
        canvas.style.width = windowWidth + "px";
        canvas.style.height = (windowWidth / gameRatio) + "px";
    } else {
        canvas.style.width = (windowHeight * gameRatio) + "px";
        canvas.style.height = windowHeight + "px";
    }
}