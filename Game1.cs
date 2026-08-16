using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cyber_Sprial
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D hotelImage, badEnding1Image, badEnding2Image,
                  cyberSpiralImage, labImage, apartmentImage, spiralFadeImage, textBoxImage;
        SpriteFont gameFont;
        private int gamePhase, subPhase, optionPressed;
        public string dialogue, question;
        public List<string> dialogueString, choiceString;
        public bool[] endingReplayDialogue = new bool[]{false, false};
        Rectangle playButtonRect, quitButtonRect;
        MouseState mState;
        Point mousePoint;
        Color fadeColor;
        bool play, mReleased, subReset, buttonPressed;
        Rectangle oneOptionRect,
                  twoOptionRect1, twoOptionRect2,
                  threeOptionRect1, threeOptionRect2, threeOptionRect3;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            gamePhase = 0;
            subPhase = 0;
            optionPressed = 0;
            dialogue = "";
            question = "";
            fadeColor = new Color(255, 255, 255, 255);
            play = false;
            mReleased = true;
            subReset = false;
            buttonPressed = false;
            dialogueString = new List<string>();
            choiceString = new List<string>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            gameFont = Content.Load<SpriteFont>("galleryFont");
            hotelImage = Content.Load<Texture2D>("hotelHallwayImage");
            badEnding1Image = Content.Load<Texture2D>("badEnd1Image");
            badEnding2Image = Content.Load<Texture2D>("badEnd2Image");
            cyberSpiralImage = Content.Load<Texture2D>("cyberSprialImage");
            labImage = Content.Load<Texture2D>("labImage");
            apartmentImage = Content.Load<Texture2D>("apartmentImage");
            spiralFadeImage = Content.Load<Texture2D>("spiralFadeImage");
            textBoxImage = Content.Load<Texture2D>("textBoxImage");
        }

        public void fade(bool fadeCheck)
        {
            //Fade out (true input)
            if (fadeCheck)
            {
                //Stop when it is fully invisible
                if (fadeColor.R != 0 && fadeColor.B != 0 && fadeColor.G != 0)
                {
                    fadeColor.R--;
                    fadeColor.G--;
                    fadeColor.B--;
                }
            }
            //Fade in (false input)
            else
            {
                //Stop once its fully visible
                if (fadeColor.R != 255 && fadeColor.B != 255 && fadeColor.B != 255)
                {
                    fadeColor.R++;
                    fadeColor.G++;
                    fadeColor.B++;
                }
            }
        }
        public bool clickCorrection()
        {
            //If clicking on screen
            bool mouseOnScreen = false;
            if ((mousePoint.X > 0 && mousePoint.X < _graphics.PreferredBackBufferWidth) &&
                (mousePoint.Y > 0 && mousePoint.X < _graphics.PreferredBackBufferHeight))
                mouseOnScreen = true;
            if (mState.LeftButton == ButtonState.Pressed && mReleased == true && mouseOnScreen)
                return true;
            //click trigger
            else if (mState.LeftButton == ButtonState.Released)
                mReleased = true;

            return false;
        }
        public void dialogueProgress(Point mousePoint)
        {
            if (clickCorrection())
            {
                subPhase++;
                mReleased = false;
            }

            //already went through the entire dialogue for the section
            if (subPhase >= dialogueString.Count)
                subReset = true;
            //else display dialogue
            else
                dialogue = dialogueString[subPhase];
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            mState = Mouse.GetState();
            mousePoint = new Point(mState.X, mState.Y);
            switch (gamePhase)
            {
                case 0:
                    //Start Screen / Pre-Game
                    
                    playButtonRect = new Rectangle(250, 125, 300, 100);
                    quitButtonRect = new Rectangle(250, 275, 300, 100);

                    if (mState.LeftButton == ButtonState.Pressed && playButtonRect.Contains(mousePoint))
                        play = true;

                    else if (mState.LeftButton == ButtonState.Pressed && quitButtonRect.Contains(mousePoint))
                        Exit();

                    if (play)
                        fade(true);

                    if (fadeColor.R == 0 && fadeColor.B == 0 && fadeColor.B == 0)
                    {
                        play = false;
                        gamePhase = 1;
                    }
                    break;
                case 1:
                    //Starting game

                    /*
                     * Once video stops, start displaying text 
                    */
                    dialogueString.Clear();
                    fade(false);
                    if(dialogueString.Count == 0)
                    {
                        dialogueString.Add("...");
                        dialogueString.Add("???");
                        dialogueString.Add("Where...");
                        dialogueString.Add("Where am I?");
                        if (endingReplayDialogue[0])
                        {
                            dialogueString.Add("How was the explosion?");
                            dialogueString.Add("You probably don't know how you are here right now");
                            dialogueString.Add("No you are not dead, but keep going...");
                            dialogueString.Add(".. and don't make that mistake again ... for your sake");
                        }
                        if (endingReplayDialogue[1])
                        {
                            dialogueString.Add("Why are you back?");
                            dialogueString.Add("I told you to stop there...");
                            dialogueString.Add("Who was that man?");
                            dialogueString.Add("Aren't you the curious one");
                            dialogueString.Add("Lets see if you can figure it out...");
                        }
                        dialogueString.Add("...");
                    }
                    if (fadeColor.R == 255 && fadeColor.B == 255 && fadeColor.B == 255)
                        dialogueProgress(mousePoint);
                    if(subReset == true)
                    {
                        //Reset
                        dialogueString.Clear();
                        subReset = false;
                        subPhase = 0;

                        //Rect call before next phase
                        choiceRect();

                        //Reseting Ending Dialogue 
                        endingReplayDialogue[0] = false;
                        endingReplayDialogue[1] = false;
                        //Next phase
                        gamePhase = 2;
                    }
                    break;
                case 2:
                    //First Choice (what do you do first)
                    question = "What do you do?";
                    //Fill choice options list
                    if (choiceString.Count == 0)
                    {
                        choiceString.Add("Investigate the Chronite");
                        choiceString.Add("Try to Remember");
                        choiceString.Add("Go to the Lab?");
                    }
                    //Option clicked
                    if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                    {
                        if (threeOptionRect1.Contains(mousePoint))
                        {
                            optionPressed = 3;
                            buttonPressed = true;
                        }
                        else if (threeOptionRect2.Contains(mousePoint))
                        {
                            optionPressed = 4;
                            buttonPressed = true;
                        }
                        else if (threeOptionRect3.Contains(mousePoint))
                        {
                            optionPressed = 5;
                            buttonPressed = true;
                        }
                        mReleased = false;
                    }
                    else if (mState.LeftButton == ButtonState.Released)
                        mReleased = true;

                    if (buttonPressed)
                    {
                        fade(true);
                        if (fadeColor.R == 0 && fadeColor.B == 0 && fadeColor.B == 0)
                        {
                            choiceString.Clear();
                            gamePhase = optionPressed;
                            buttonPressed = false;
                        }
                    }

                    break;
                case 3:
                    //Bad Ending 1 (Bad Chronite)
                    fade(false);
                    if (dialogueString.Count == 0)
                    {
                        dialogueString.Add("...");
                        dialogueString.Add("Really?");
                        dialogueString.Add("You touched the first shiny blue gem-like thing you see?");
                        dialogueString.Add("It's called \'Chronite\' on the option you just pressed");
                        dialogueString.Add("And now, Guess what?");
                        dialogueString.Add("You died");
                        dialogueString.Add("That Chronite exploded on you");
                        dialogueString.Add("Bad Ending 1 \"Death by Chronite\"");
                        dialogueString.Add("(Click to restart)");
                    }
                    if (fadeColor.R == 255 && fadeColor.B == 255 && fadeColor.B == 255)
                        dialogueProgress(mousePoint);
                    if (subReset == true)
                    {
                        //Reset
                        dialogueString.Clear();
                        subReset = false;
                        subPhase = 0;

                        //Ending Dialogue Enabled for Restart
                        endingReplayDialogue[0] = true;

                        //Next phase
                        gamePhase = 0;
                    }
                    break;
                case 4:
                    //Dialouge about what you remember
                    fade(false);
                    if (dialogueString.Count == 0)
                    {
                        dialogueString.Add("...");
                        dialogueString.Add("What do you remember?");
                        dialogueString.Add("(Insert dialogue about details about the room, when the art is done)");
                        dialogueString.Add("You try and remember, but nothing about this room is familiar");
                        dialogueString.Add("...");
                    }
                    if (fadeColor.R == 255 && fadeColor.B == 255 && fadeColor.B == 255)
                        dialogueProgress(mousePoint);
                    if (subReset == true)
                    {
                        //Reset
                        dialogueString.Clear();
                        subReset = false;
                        subPhase = 0;

                        //Next phase
                        gamePhase = 5;
                    }
                    break;
                case 5:
                    //Go to Lab
                    fade(false);
                    if (dialogueString.Count == 0)
                    {
                        dialogueString.Add("Just outside your vision you see a light through a crack...");
                        dialogueString.Add("You investigate that light and it is a hidden door behind the wall");
                        dialogueString.Add("Which reveals a staircase down to a lab!");
                        dialogueString.Add("You start to think, \"Is this my lab?\" ");
                        dialogueString.Add("Try as you may you can't remember anything about a lab in your life");
                        dialogueString.Add("You also notice you can't remember anything past a few months ago");
                        dialogueString.Add("...");
                        dialogueString.Add("Strange");
                        dialogueString.Add("Very Strange...");
                    }
                    if (fadeColor.R == 255 && fadeColor.B == 255 && fadeColor.B == 255)
                        dialogueProgress(mousePoint);
                    if (subReset == true)
                    {
                        //Reset
                        dialogueString.Clear();
                        subReset = false;
                        subPhase = 0;

                        //Next phase
                        gamePhase = 6;
                    }
                    break;
                case 6:
                    //Intro dialogue with entering lab
                    if (dialogueString.Count == 0)
                    {
                        dialogueString.Add("While you are investigating the lab, you don't find anything too interesting");
                        dialogueString.Add("Just some papers, bottles, unidentifed liquids you probably shouldn't drink, and some more of that weird \"Chronite\"");
                        dialogueString.Add("After a few minutes of exploring the lab you hear a knocking sound");
                        dialogueString.Add("Is someone at the front door?");
                    }
                    if (fadeColor.R == 255 && fadeColor.B == 255 && fadeColor.B == 255)
                        dialogueProgress(mousePoint);
                    if (subReset == true)
                    {
                        //Reset
                        dialogueString.Clear();
                        subReset = false;
                        subPhase = 0;

                        //Next phase
                        gamePhase = 7;
                    }
                    break;
                case 7:
                    //Descision 2 (Upstairs there is a knock at the door)
                    question = "What will you do?";
                    //Fill choice options list
                    if (choiceString.Count == 0)
                    {
                        choiceString.Add("Don't Answer it");
                        choiceString.Add("Answer it");
                    }
                    //Option clicked
                    if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                    {
                        if (twoOptionRect1.Contains(mousePoint))
                        {
                            optionPressed = 8;
                            buttonPressed = true;
                        }
                        else if (twoOptionRect2.Contains(mousePoint))
                        {
                            optionPressed = 9;
                            buttonPressed = true;
                        }
                        mReleased = false;
                    }
                    else if (mState.LeftButton == ButtonState.Released)
                        mReleased = true;

                    if (buttonPressed)
                    {
                        fade(true);
                        if (fadeColor.R == 0 && fadeColor.B == 0 && fadeColor.B == 0)
                        {
                            buttonPressed = false;
                            choiceString.Clear();
                            gamePhase = optionPressed;
                            
                        }
                    }
                    break;
                case 8:
                    //Don't answer the door
                    fade(false);
                    if (dialogueString.Count == 0)
                    {
                        dialogueString.Add("(You don't answer the door)");
                        dialogueString.Add("Guess you will never know what...");
                        dialogueString.Add("or who... was behind the door");
                    }
                    if (fadeColor.R == 255 && fadeColor.B == 255 && fadeColor.B == 255)
                        dialogueProgress(mousePoint);
                    if (subReset == true)
                    {
                        //Reset
                        dialogueString.Clear();
                        subReset = false;
                        subPhase = 0;

                        //Next phase
                        gamePhase = 14;
                    }
                    break;
                case 9:
                    //Answer the door
                    fade(false);
                    if (dialogueString.Count == 0)
                    {
                        dialogueString.Add("You run back up the lab staircase");
                        dialogueString.Add("Close the hidden door behind you");
                        dialogueString.Add("Then you peek through the door hole");
                        dialogueString.Add("You don't see anyone");
                        dialogueString.Add("But your curiousity got the best of you");
                        dialogueString.Add("and you still open the door anyway");
                        dialogueString.Add("...");
                    }
                    if (fadeColor.R == 255 && fadeColor.B == 255 && fadeColor.B == 255)
                        dialogueProgress(mousePoint);
                    if (subReset == true)
                    {
                        //Reset
                        dialogueString.Clear();
                        subReset = false;
                        subPhase = 0;

                        //Next phase
                        gamePhase = 10;
                    }
                    break;
                case 10:
                    //Choice 3 (Explore or no)
                    question = "What do you choose?";
                    //Fill choice options list
                    if (choiceString.Count == 0)
                    {
                        choiceString.Add("Explore the Hallway");
                        choiceString.Add("Stay inside");
                    }
                    //Option clicked
                    if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                    {
                        if (twoOptionRect1.Contains(mousePoint))
                        {
                            optionPressed = 11;
                            buttonPressed = true;
                        }
                        else if (twoOptionRect2.Contains(mousePoint))
                        {
                            optionPressed = 12;
                            buttonPressed = true;
                        }
                        mReleased = false;
                    }
                    else if (mState.LeftButton == ButtonState.Released)
                        mReleased = true;

                    if (buttonPressed)
                    {
                        fade(true);
                        if (fadeColor.R == 0 && fadeColor.B == 0 && fadeColor.B == 0)
                        {
                            choiceString.Clear();
                            gamePhase = optionPressed;
                            buttonPressed = false;
                        }
                    }
                    break;
                case 11:
                    //Explore hallway
                    fade(false);
                    if (dialogueString.Count == 0)
                    {
                        dialogueString.Add("You open the door, slowly...");
                        dialogueString.Add("very slowly...");
                        dialogueString.Add("At first glance, it looks like a normal hallway");
                        dialogueString.Add("It appears you are in a hotel");
                        dialogueString.Add("(\'How does a lab with a staircase work in a hotel?!\')");
                        dialogueString.Add("As you step further into the door to get a better look around the hallway");
                        dialogueString.Add("A man dashing in front of you, pushes you on the ground in your room, and closes the door behind him");
                        dialogueString.Add("Strange man: \'I was hoping you weren't alive in there, the job we hired you for ended already, you just haven't finished the job yourself\'");
                        dialogueString.Add("Strange man: \'I was always curious how you did the things that you did, some could say I was a fan\'");
                        dialogueString.Add("Strange man: \'You did good, even great for us, but you should have died with that job\'");
                    }
                    if (fadeColor.R == 255 && fadeColor.B == 255 && fadeColor.B == 255)
                        dialogueProgress(mousePoint);
                    if (subReset == true)
                    {
                        //Reset
                        dialogueString.Clear();
                        subReset = false;
                        subPhase = 0;

                        //Next phase
                        gamePhase = 13;
                    }
                    break;
                case 12:
                    //Go back inside
                    fade(false);
                    if (dialogueString.Count == 0)
                    {
                        dialogueString.Add("You kept the door open just enough to see that there isn't anything too interesting to explore in the hallway");
                        dialogueString.Add("Probably kids or something doing ding dong ditch, or realising the wrong room too late...");
                        dialogueString.Add("Hopefully...");
                        dialogueString.Add("Probably...");
                        dialogueString.Add("...");
                        dialogueString.Add("...");
                        dialogueString.Add("...");
                        dialogueString.Add("Anyways");
                    }
                    if (fadeColor.R == 255 && fadeColor.B == 255 && fadeColor.B == 255)
                        dialogueProgress(mousePoint);
                    if (subReset == true)
                    {
                        //Reset
                        dialogueString.Clear();
                        subReset = false;
                        subPhase = 0;

                        //Next phase
                        gamePhase = 14;
                    }
                    break;
                case 13:
                    //Bad Ending 2
                    fade(false);
                    if (dialogueString.Count == 0)
                    {
                        dialogueString.Add("...");
                        dialogueString.Add("^ that is the last thing you saw");
                        dialogueString.Add("Yeah...");
                        dialogueString.Add("That... Just... Happened...");
                        dialogueString.Add("He totally wasn't important or anything to help you remember any and all things...");
                        dialogueString.Add("Take his advice, you have gone far enough...");
                        dialogueString.Add("When you restart... Don't");
                        dialogueString.Add("(\'Don't\' Click to restart)");
                    }
                    if (fadeColor.R == 255 && fadeColor.B == 255 && fadeColor.B == 255)
                        dialogueProgress(mousePoint);
                    if (subReset == true)
                    {
                        //Reset
                        dialogueString.Clear();
                        subReset = false;
                        subPhase = 0;

                        //Ending Dialogue Enabled for Restart
                        endingReplayDialogue[1] = true;

                        //Next phase
                        gamePhase = 0;
                    }
                    break;
                case 14:
                    //"Ending"
                    if (dialogueString.Count == 0)
                    {
                        dialogueString.Add("The End?");
                        dialogueString.Add("Yeah, what else did you expect?");
                        dialogueString.Add("Did you make sure you made the right choices?");
                        dialogueString.Add("Does any of this resonate with you? or you \'The Player\'");
                        dialogueString.Add("Yeah, you playing the game. I can see you");
                        dialogueString.Add("Still confused?, Fine");
                        dialogueString.Add("Lets try that again");
                    }
                    dialogueProgress(mousePoint);
                    if (subReset == true)
                    {
                        //Reset
                        dialogueString.Clear();
                        subReset = false;
                        subPhase = 0;

                        //Next phase
                        gamePhase = 0;
                    }
                    break;
                default:
                    Debug.WriteLine("You did it, you managed to break my code and my game");
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            switch (gamePhase)
            {
                case 0:
                    //Start Screen / Pre-Game
                    //Title and two images next to title
                    _spriteBatch.DrawString(gameFont,"Cyber-Sprial",new Vector2(325,10), Color.Cyan);
                    _spriteBatch.Draw(cyberSpiralImage, new Rectangle(250,0,75,75), fadeColor);
                    _spriteBatch.Draw(cyberSpiralImage, new Rectangle(500, 0, 75, 75), fadeColor);

                    //Buttons
                    _spriteBatch.Draw(cyberSpiralImage, playButtonRect, fadeColor);
                    _spriteBatch.Draw(labImage, quitButtonRect, fadeColor);

                    //Dev Text
                    _spriteBatch.DrawString(gameFont, "Deveopled By: Michael McLaughlin & Nicholas Cordial", new Vector2(25, 445), fadeColor);
                    break;
                case 1: 
                    //Starting game
                    //Insert Video HEREEEEEEEE
                    imageFormat(apartmentImage, true);
                    textBox(gameFont, textBoxImage, fadeColor);
                    break;
                case 2:
                    //First Choice (what do you do first)
                    choiceSetup(gameFont, labImage, fadeColor, choiceString.Count, question);
                    break;
                case 3:
                    //Bad Ending 1 (Bad Chronite)
                    imageFormat(badEnding1Image, false);
                    textBox(gameFont, textBoxImage, fadeColor);
                    break;
                case 5: case 6:
                    //Dialouge about what you remember 
                    //Go to Lab
                    //Intro dialogue with entering lab
                    imageFormat(labImage, true);
                    textBox(gameFont, textBoxImage, fadeColor);
                    break;
                case 7:
                    //Descision 2 (Upstairs there is a knock at the door)
                    choiceSetup(gameFont, labImage, fadeColor, choiceString.Count, question);
                    break;
                case 4: case 8: case 9:
                    //Don't answer the door
                    //Answer the door
                    imageFormat(apartmentImage, true);
                    textBox(gameFont, textBoxImage, fadeColor);
                    break;
                case 10:
                    //Choice 3 (Explore or no)
                    choiceSetup(gameFont, labImage, fadeColor, choiceString.Count, question);
                    break;
                case 11: case 12:
                    //Explore hallway
                    //Go back inside
                    imageFormat(hotelImage, true);
                    textBox(gameFont, textBoxImage, fadeColor);
                    break;
                case 13:
                    //Bad Ending 2
                    imageFormat(badEnding2Image, false);
                    textBox(gameFont, textBoxImage, fadeColor);
                    break;
                case 14:
                    //"Ending"
                    textBox(gameFont, textBoxImage, fadeColor);
                    break;
                default:
                    Debug.WriteLine("You did it, you managed to break my drawings and my game");
                    break;
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void imageFormat(Texture2D imageTexture, bool DemoImage)
        {
            if(DemoImage)
                _spriteBatch.Draw(imageTexture,
                        new Rectangle(15, 0, 775, 300),
                        fadeColor);
            else
                _spriteBatch.Draw(imageTexture,
                    new Rectangle(0,0,imageTexture.Width,imageTexture.Height+80),
                    fadeColor);
        }
        public void textBox(SpriteFont font, Texture2D boxTexture, Color fadeColor)
        {
            Vector2 pos = new (0,0);
            Rectangle textboxRect = new Rectangle(
                (int)pos.X,
                (int)pos.Y,
                (_graphics.PreferredBackBufferWidth),
                (_graphics.PreferredBackBufferHeight));
            _spriteBatch.Draw(boxTexture, textboxRect, fadeColor);
            _spriteBatch.DrawString(font, WrapText(font, dialogue, (textboxRect.X + textboxRect.Width)-10),
                new Vector2(pos.X + 15, ((_graphics.PreferredBackBufferHeight*5)/8)+15), fadeColor);
        }

        public void choiceRect()
        {
            oneOptionRect = new Rectangle(250, 200, 300, 100);

            twoOptionRect1 = new Rectangle(250, 125, 300, 100);
            twoOptionRect2 = new Rectangle(250, 275, 300, 100);

            threeOptionRect1 = new Rectangle(400, 125, 300, 100);
            threeOptionRect2 = new Rectangle(25, ((_graphics.PreferredBackBufferHeight) / 2), 300, 100);
            threeOptionRect3 = new Rectangle(450, ((_graphics.PreferredBackBufferHeight) * 3 / 4), 300, 100);
        }
        public void choiceSetup(SpriteFont font, Texture2D boxTexture, Color fadeColor, int numChoice, string question)
        {
            //Question Box
            Vector2 questionPos = new Vector2((_graphics.PreferredBackBufferWidth) / 4, 0);
            Rectangle questionBox = new Rectangle(
                (int)questionPos.X,
                (int)questionPos.Y,
                (_graphics.PreferredBackBufferWidth) / 2,
                75);
            _spriteBatch.Draw(boxTexture, questionBox, fadeColor);
            _spriteBatch.DrawString(font, question, new Vector2((questionPos.X+(questionBox.Width)/5), (questionBox.Height) / 4), fadeColor);

            //Options and how many
            switch (numChoice)
            {
                case 1:
                    choiceTextAndBox(oneOptionRect, boxTexture, 0);
                    break;
                case 2:
                    choiceTextAndBox(twoOptionRect1, boxTexture, 0);
                    choiceTextAndBox(twoOptionRect2, boxTexture, 1);
                    break;
                case 3:
                    choiceTextAndBox(threeOptionRect1, boxTexture, 0);
                    choiceTextAndBox(threeOptionRect2, boxTexture, 1);
                    choiceTextAndBox(threeOptionRect3, boxTexture, 2);
                    break;
                default:
                    break;
            }
        }

        public void choiceTextAndBox(Rectangle optionRect, Texture2D boxTexture, int optionCount)
        {
            //drawing each option
            _spriteBatch.Draw(boxTexture, optionRect, fadeColor);
            _spriteBatch.DrawString(gameFont,
                WrapText(gameFont, choiceString[optionCount], (optionRect.X + optionRect.Width)),
                new Vector2(optionRect.X + 5, (optionRect.Y) + (optionRect.Height / 4)),
                fadeColor);
        }

        public static string WrapText(SpriteFont font, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = font.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = font.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    if (size.X > maxLineWidth)
                    {
                        if (sb.ToString() == "")
                            sb.Append(WrapText(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                        else
                            sb.Append("\n" + WrapText(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                    }
                    else
                    {
                        sb.Append("\n" + word + " ");
                        lineWidth = size.X + spaceWidth;
                    }
                }
            }
            return sb.ToString();
        }
    }
}
