using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Shmup
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D saucerTxr, missileTxr, backgroundTxr, particleTxr;
        Point screenSize = new Point(800, 450);
        float spawnCooldown = 2;
        float playTime = 0;

        


        Sprite backgroundSprite;
        PlayerSprite playerSprite;
        List<MissileSprite> missileList = new List<MissileSprite>();
        List<ParticleSprite> particleList = new List<ParticleSprite>();
        SpriteFont uiFont, bigFont;
        Song bossBattle;
        SoundEffect missileExplode, shipExplode;
        
        


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = screenSize.X;
            _graphics.PreferredBackBufferHeight = screenSize.Y;
            _graphics.ApplyChanges();
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            saucerTxr = Content.Load<Texture2D>("saucer");
            missileTxr = Content.Load<Texture2D>("missile");
            backgroundTxr = Content.Load<Texture2D>("background");
            particleTxr = Content.Load<Texture2D>("particle");
            uiFont = Content.Load<SpriteFont>("UIFont");
            bigFont = Content.Load<SpriteFont>("BigFont");
            backgroundSprite = new Sprite(backgroundTxr, new Vector2());
            bossBattle = Content.Load<Song>("Boss Battle!!");
            missileExplode = Content.Load<SoundEffect>("Explosion");
            shipExplode = Content.Load<SoundEffect>("shipExplode");
            playerSprite = new PlayerSprite(saucerTxr, new Vector2(screenSize.X / 6, screenSize.Y / 2));
            MediaPlayer.Play(bossBattle);



            
            
            
            // TODO: use this.Content to load your game content here
        }
        
        protected override void Update(GameTime gameTime)
        {

            Random rng = new Random();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            if (spawnCooldown > 0)
            {
              
                spawnCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }



            else if (playerSprite.playerLives > 0 && missileList.Count < 5)
            {
                missileList.Add(new MissileSprite(missileTxr, new Vector2(screenSize.X, rng.Next(0, screenSize.Y - missileTxr.Height)), (Math.Min(playTime, 60f)/60) * 20000f + 200f));
                spawnCooldown = (float)rng.NextDouble() + 0.5f; 
            }

            if (playerSprite.playerLives > 0)
            {
                playerSprite.Update(gameTime, screenSize);
                playTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            foreach (MissileSprite missile in missileList)
            {
                missile.Update(gameTime, screenSize);
                playerSprite.IsColliding(missile);
                if  (playerSprite.playerLives > 0 && playerSprite.IsColliding(missile))
                {
                 for (int i = 0; i < 16; i++)   particleList.Add(new ParticleSprite(particleTxr, new Vector2 (missile.spritePos.X + (missileTxr.Width / 2) - (particleTxr.Width / 2), missile.spritePos.Y + (missileTxr.Height / 2) - (particleTxr.Height / 2))));
                    missile.dead = true;
                    playerSprite.playerLives--;
                    missileExplode.Play();
                    if (playerSprite.playerLives == 0) 
                    {
                        for (int i = 0; i < 32; i++) particleList.Add(new ParticleSprite(particleTxr, new Vector2(playerSprite.spritePos.X + (saucerTxr.Width / 2) - (particleTxr.Width / 2), playerSprite.spritePos.Y + (saucerTxr.Height / 2) - (particleTxr.Height / 2))));
                    }
                    shipExplode.Play();
                }
            }

            foreach (ParticleSprite particle in particleList) particle.Update(gameTime, screenSize);

            missileList.RemoveAll(missile => missile.dead);
            particleList.RemoveAll(particle => particle.currentLife <= 0);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            _spriteBatch.Begin();

            backgroundSprite.Draw(_spriteBatch);

                       
            if (playerSprite.playerLives > 0) playerSprite.Draw(_spriteBatch);



            foreach (MissileSprite missile in missileList) missile.Draw(_spriteBatch);
            foreach (ParticleSprite particle in particleList) particle.Draw(_spriteBatch);
            _spriteBatch.DrawString(uiFont, "Lives: " + playerSprite.playerLives, new Vector2(12, 12), Color.Black);
            _spriteBatch.DrawString(uiFont, "Lives: " + playerSprite.playerLives, new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(uiFont, "Time:: " + Math.Round(playTime), new Vector2(16, 48), Color.Black);
            _spriteBatch.DrawString(uiFont, "Time: " +  Math.Round(playTime), new Vector2(16, 46), Color.White);

            if (playerSprite.playerLives <= 0) 
            {
                Vector2 textSize = bigFont.MeasureString("GAME OVER!");
                _spriteBatch.DrawString(bigFont, "GAME OVER!", new Vector2((screenSize.X / 2) - (textSize.X /2), (screenSize.Y / 2) - (textSize.Y / 2)), Color.White);
                _spriteBatch.DrawString(bigFont, "GAME OVER!", new Vector2((screenSize.X / 2+8) - (textSize.X / 2+8), (screenSize.Y / 2+8) - (textSize.Y / 2+8)), Color.Black);

                MediaPlayer.Stop(); 
            }






            _spriteBatch.End();



            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

    }
}
