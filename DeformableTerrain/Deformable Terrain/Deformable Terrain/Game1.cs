using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Deformable_Terrain
{
  /// <summary>
  /// This is the main type for your game
  /// </summary>
  public class Game1 : Microsoft.Xna.Framework.Game
  {
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    private Texture2D textureSky;
    private Texture2D textureLevel;
    private Texture2D textureDeform;

    private Vector2 mousePosition;

    private MouseState currentMouseState;
    private uint[] pixelDeformData;

    

    public Game1()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      this.IsMouseVisible = true;

      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      spriteBatch = new SpriteBatch(GraphicsDevice);

      textureSky = Content.Load<Texture2D>("sky");
      textureLevel = Content.Load<Texture2D>("level");
      textureDeform = Content.Load<Texture2D>("deform");

      // Declare an array to hold the pixel data
      pixelDeformData = new uint[textureDeform.Width * textureDeform.Height];
      // Populate the array
      textureDeform.GetData(pixelDeformData, 0, textureDeform.Width * textureDeform.Height);
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      // Allows the game to exit
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
        this.Exit();

      UpdateMouse();

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

      spriteBatch.Begin();
      
      spriteBatch.Draw(textureSky, new Vector2(0, 0), Color.White);
      spriteBatch.Draw(textureLevel, new Vector2(0, 0), Color.White);
      spriteBatch.Draw(textureDeform, mousePosition, Color.White);
      
      spriteBatch.End();


      base.Draw(gameTime);
    }

    protected void UpdateMouse()
    {
      MouseState previousMouseState = currentMouseState;
      
      currentMouseState = Mouse.GetState();

      // This gets the mouse co-ordinates
      // relative to the upper left of the game window
      mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

      // Here we make sure that we only call the deform level function
      // when the left mouse button is released
      if (previousMouseState.LeftButton == ButtonState.Pressed &&
        currentMouseState.LeftButton == ButtonState.Released)
      {
        DeformLevel();
      }
    }

    /// <summary>
    /// 16777215 = Alpha
    /// 4294967295 = White
    /// </summary>
    protected void DeformLevel()
    {
      // Declare an array to hold the pixel data
      uint[] pixelLevelData = new uint[textureLevel.Width * textureLevel.Height];
      // Populate the array
      textureLevel.GetData(pixelLevelData, 0, textureLevel.Width * textureLevel.Height);

      for (int x = 0; x < textureDeform.Width; x++)
      {
        for (int y = 0; y < textureDeform.Height; y++)
        {          
          // Do some error checking so we dont draw out of bounds of the array etc..
          if (((mousePosition.X + x) < (textureLevel.Width)) && 
            ((mousePosition.Y + y) < (textureLevel.Height)))
          {
            if ((mousePosition.X + x) >= 0 && (mousePosition.Y + y) >= 0)
            {
              // Here we check that the current co-ordinate of the deform texture is not an alpha value
              // And that the current level texture co-ordinate is not an alpha value
              if (pixelDeformData[x + y * textureDeform.Width] != 16777215 
                && pixelLevelData[((int)mousePosition.X + x) + 
                ((int)mousePosition.Y + y) * textureLevel.Width] != 16777215)
              {
                // We then check to see if the deform texture's current pixel is white (4294967295)                
                if (pixelDeformData[x + y * textureDeform.Width] == 4294967295)
                {
                  // It's white so we replace it with an Alpha pixel
                  pixelLevelData[((int)mousePosition.X + x) + ((int)mousePosition.Y + y)
                    * textureLevel.Width] = 16777215;
                }
                else
                {
                  // Its not white so just set the level texture pixel to the deform texture pixel
                  pixelLevelData[((int)mousePosition.X + x) + ((int)mousePosition.Y + y) 
                    * textureLevel.Width] = pixelDeformData[x + y * textureDeform.Width];
                }
              }
            }
          }
        }
      }

      // Update the texture with the changes made above
      textureLevel.SetData(pixelLevelData);
    }
 
  }
}
