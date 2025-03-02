using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace RandoSeed
{
    public partial class MainApp : Form
    {
        // Variables to handle dragging the form
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        // Timer for fade-out effect
        private Timer fadeOutTimer;
        private const int fadeOutInterval = 1; // Interval between opacity changes in milliseconds
        private const double opacityDecrement = 0.1; // Amount to decrease opacity each interval

        private Timer zoomTimer;
        private const int zoomInterval = 10; // Interval for zoom effect in milliseconds
        private const float zoomIncrement = 0.05f; // Increment for zoom effect
        private bool isZoomingIn = false;
        private bool isZoomingOut = false;

        public MainApp()
        {
            InitializeComponent();
            buttonClickInitialisation();

            this.FormBorderStyle = FormBorderStyle.None; // Remove the form border to allow custom shapes
            this.Load += MainApp_Load; // Subscribe to the Load event

            // Add event handlers for NavDraggable to enable dragging the form
            this.NavDraggable.MouseDown += new MouseEventHandler(NavDraggable_MouseDown);
            this.NavDraggable.MouseMove += new MouseEventHandler(NavDraggable_MouseMove);
            this.NavDraggable.MouseUp += new MouseEventHandler(NavDraggable_MouseUp);

            // Initialize the fade-out timer
            fadeOutTimer = new Timer();
            fadeOutTimer.Interval = fadeOutInterval;
            fadeOutTimer.Tick += FadeOutTimer_Tick;

            // Initialize the clipboard timer
            clipboardTimer = new Timer();
            clipboardTimer.Interval = clipboardVisibleDuration;
            clipboardTimer.Tick += ClipboardTimer_Tick;

            // Initialize the zoom timer
            zoomTimer = new Timer();
            zoomTimer.Interval = zoomInterval;
            zoomTimer.Tick += ZoomTimer_Tick;
        }

        private void MainApp_Load(object sender, EventArgs e)
        {
            this.Hide(); // Hide the form during initialization
            initialiseGraphics(); // Initialize custom graphics
            SetCustomFont();
            this.Show(); // Show the form after initialization
        }

        private void SetCustomFont()
        {
            // Load the font from resources
            PrivateFontCollection privateFonts = new PrivateFontCollection();
            int fontLength = Properties.Resources.Ubuntu_Bold.Length;
            byte[] fontData = Properties.Resources.Ubuntu_Bold;
            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontLength);
            Marshal.Copy(fontData, 0, fontPtr, fontLength);
            privateFonts.AddMemoryFont(fontPtr, fontLength);
            Marshal.FreeCoTaskMem(fontPtr);

            // Set the custom font to textOutput with explicit FontStyle
            textOutput.Font = new Font(privateFonts.Families[0], 12.0F, FontStyle.Bold);
        }

        private void buttonClickInitialisation()
        {
            // Add click event handlers for option buttons
            this.optionOneButton.Click += new System.EventHandler(this.optionButton_Click);
            this.optionTwoButton.Click += new System.EventHandler(this.optionButton_Click);
            this.optionThreeButton.Click += new System.EventHandler(this.optionButton_Click);
            this.optionFourButton.Click += new System.EventHandler(this.optionButton_Click);
            this.optionFiveButton.Click += new System.EventHandler(this.optionButton_Click);
            this.optionSixButton.Click += new System.EventHandler(this.optionButton_Click);
            this.optionSevenButton.Click += new System.EventHandler(this.optionButton_Click);
            this.optionEightButton.Click += new System.EventHandler(this.optionButton_Click);
            this.optionNineButton.Click += new System.EventHandler(this.optionButton_Click);
            this.optionTenButton.Click += new System.EventHandler(this.optionButton_Click);

            // Add mouse enter and leave event handlers for option buttons
            this.optionOneButton.MouseEnter += new System.EventHandler(this.optionButton_MouseEnter);
            this.optionOneButton.MouseLeave += new System.EventHandler(this.optionButton_MouseLeave);
            this.optionTwoButton.MouseEnter += new System.EventHandler(this.optionButton_MouseEnter);
            this.optionTwoButton.MouseLeave += new System.EventHandler(this.optionButton_MouseLeave);
            this.optionThreeButton.MouseEnter += new System.EventHandler(this.optionButton_MouseEnter);
            this.optionThreeButton.MouseLeave += new System.EventHandler(this.optionButton_MouseLeave);
            this.optionFourButton.MouseEnter += new System.EventHandler(this.optionButton_MouseEnter);
            this.optionFourButton.MouseLeave += new System.EventHandler(this.optionButton_MouseLeave);
            this.optionFiveButton.MouseEnter += new System.EventHandler(this.optionButton_MouseEnter);
            this.optionFiveButton.MouseLeave += new System.EventHandler(this.optionButton_MouseLeave);
            this.optionSixButton.MouseEnter += new System.EventHandler(this.optionButton_MouseEnter);
            this.optionSixButton.MouseLeave += new System.EventHandler(this.optionButton_MouseLeave);
            this.optionSevenButton.MouseEnter += new System.EventHandler(this.optionButton_MouseEnter);
            this.optionSevenButton.MouseLeave += new System.EventHandler(this.optionButton_MouseLeave);
            this.optionEightButton.MouseEnter += new System.EventHandler(this.optionButton_MouseEnter);
            this.optionEightButton.MouseLeave += new System.EventHandler(this.optionButton_MouseLeave);
            this.optionNineButton.MouseEnter += new System.EventHandler(this.optionButton_MouseEnter);
            this.optionNineButton.MouseLeave += new System.EventHandler(this.optionButton_MouseLeave);
            this.optionTenButton.MouseEnter += new System.EventHandler(this.optionButton_MouseEnter);
            this.optionTenButton.MouseLeave += new System.EventHandler(this.optionButton_MouseLeave);

            // Add mouse enter and leave event handlers for toggleOptionsButton
            this.toggleOptionsButton.MouseEnter += new System.EventHandler(this.toggleOptionsButton_MouseEnter);
            this.toggleOptionsButton.MouseLeave += new System.EventHandler(this.toggleOptionsButton_MouseLeave);

            // Add click event handlers for radio buttons
            this.optionOneRadio.Click += new System.EventHandler(this.radioButton_Click);
            this.optionTwoRadio.Click += new System.EventHandler(this.radioButton_Click);
            this.optionThreeRadio.Click += new System.EventHandler(this.radioButton_Click);
            this.optionFourRadio.Click += new System.EventHandler(this.radioButton_Click);
            this.optionFiveRadio.Click += new System.EventHandler(this.radioButton_Click);
            this.optionSixRadio.Click += new System.EventHandler(this.radioButton_Click);

            // Add mouse enter and leave event handlers for radio buttons
            this.optionOneRadio.MouseEnter += new System.EventHandler(this.radioButton_MouseEnter);
            this.optionOneRadio.MouseLeave += new System.EventHandler(this.radioButton_MouseLeave);
            this.optionTwoRadio.MouseEnter += new System.EventHandler(this.radioButton_MouseEnter);
            this.optionTwoRadio.MouseLeave += new System.EventHandler(this.radioButton_MouseLeave);
            this.optionThreeRadio.MouseEnter += new System.EventHandler(this.radioButton_MouseEnter);
            this.optionThreeRadio.MouseLeave += new System.EventHandler(this.radioButton_MouseLeave);
            this.optionFourRadio.MouseEnter += new System.EventHandler(this.radioButton_MouseEnter);
            this.optionFourRadio.MouseLeave += new System.EventHandler(this.radioButton_MouseLeave);
            this.optionFiveRadio.MouseEnter += new System.EventHandler(this.radioButton_MouseEnter);
            this.optionFiveRadio.MouseLeave += new System.EventHandler(this.radioButton_MouseLeave);
            this.optionSixRadio.MouseEnter += new System.EventHandler(this.radioButton_MouseEnter);
            this.optionSixRadio.MouseLeave += new System.EventHandler(this.radioButton_MouseLeave);
        }



        private void initialiseGraphics()
        {
            // Set the form's region to a rounded rectangle
            using (GraphicsPath path = GetRoundedRectPath(this.ClientRectangle, 12))
            {
                this.Region = new Region(path);
            }

            // Set the background of the buttons to transparent
            closeButton.BackColor = System.Drawing.Color.Transparent;
            minimiseButton.BackColor = System.Drawing.Color.Transparent;
            copyButton.BackColor = System.Drawing.Color.Transparent;
            startButton.BackColor = System.Drawing.Color.Transparent;
            clipboardNotif.BackColor = System.Drawing.Color.Transparent;

            // Set the parent of the buttons to NavDraggable to ensure transparency
            closeButton.Parent = NavDraggable;
            minimiseButton.Parent = NavDraggable;
            copyButton.Parent = backFrame;
            startButton.Parent = backFrame;
            clipboardNotif.Parent = backFrame;
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            // Create a GraphicsPath with rounded corners
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void minimiseButton_Click(object sender, EventArgs e)
        {
            // Minimize the form
            this.WindowState = FormWindowState.Minimized;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            // Start the fade-out timer
            fadeOutTimer.Start();
        }

        private void FadeOutTimer_Tick(object sender, EventArgs e)
        {
            // Decrease the form's opacity
            this.Opacity -= opacityDecrement;

            // Check if the form is fully transparent
            if (this.Opacity <= 0)
            {
                // Stop the timer and close the form
                fadeOutTimer.Stop();
                this.Close();
            }
        }

        private void NavDraggable_MouseDown(object sender, MouseEventArgs e)
        {
            // Start dragging the form
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void NavDraggable_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                // Calculate the new position of the form
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }

        private void NavDraggable_MouseUp(object sender, MouseEventArgs e)
        {
            // Stop dragging the form
            dragging = false;
        }

        private void eyeButton_Click(object sender, EventArgs e)
        {
            if (eyeButton.Tag as string == "eye_open")
            {
                textOutput.UseSystemPasswordChar = true;
                eyeButton.Image = Properties.Resources.eye_closed;
                eyeButton.Tag = "eye_closed";  // Update tag to reflect current state
            }
            else
            {
                textOutput.UseSystemPasswordChar = false;
                eyeButton.Image = Properties.Resources.eye;
                eyeButton.Tag = "eye_open";  // Update tag to reflect current state
            }
        }

        private void eyeButton_MouseEnter(object sender, EventArgs e)
        {
            eyeButton.Image = Properties.Resources.eye_hover;
        }

        private void eyeButton_MouseLeave(object sender, EventArgs e)
        {
            if (eyeButton.Tag as string == "eye_open")
            {
                eyeButton.Image = Properties.Resources.eye;
            }
            else
            {
                eyeButton.Image = Properties.Resources.eye_closed;
            }
        }

        private void toggleOptionsButton_Click(object sender, EventArgs e)
        {
            // Determine the new state and image based on the current state of the toggle button
            bool toggleOn = (toggleOptionsButton.Tag as string == "toggle_off");
            Image newImage = toggleOn ? Properties.Resources.CheckboxSelected : Properties.Resources.CheckboxNotSelected;
            string newTag = toggleOn ? "selected" : "not_selected";

            // Update the tag of the toggle button to reflect the new state
            toggleOptionsButton.Tag = toggleOn ? "toggle_on" : "toggle_off";

            // Update the images and tags of all option buttons
            toggleOptionsButton.Image = newImage;
            optionOneButton.Image = newImage;
            optionOneButton.Tag = newTag;
            optionTwoButton.Image = newImage;
            optionTwoButton.Tag = newTag;
            optionThreeButton.Image = newImage;
            optionThreeButton.Tag = newTag;
            optionFourButton.Image = newImage;
            optionFourButton.Tag = newTag;
            optionFiveButton.Image = newImage;
            optionFiveButton.Tag = newTag;
            optionSixButton.Image = newImage;
            optionSixButton.Tag = newTag;
            optionSevenButton.Image = newImage;
            optionSevenButton.Tag = newTag;
            optionEightButton.Image = newImage;
            optionEightButton.Tag = newTag;
            optionNineButton.Image = newImage;
            optionNineButton.Tag = newTag;
            optionTenButton.Image = newImage;
            optionTenButton.Tag = newTag;
        }


        private void optionButton_Click(object sender, EventArgs e)
        {
            PictureBox optionButton = sender as PictureBox;
            if (optionButton != null)
            {
                if (optionButton.Tag as string == "selected")
                {
                    optionButton.Image = Properties.Resources.CheckboxNotSelected;
                    optionButton.Tag = "not_selected";
                }
                else
                {
                    optionButton.Image = Properties.Resources.CheckboxSelected;
                    optionButton.Tag = "selected";
                }
            }
        }

        private void radioButton_Click(object sender, EventArgs e)
        {
            PictureBox clickedRadioButton = sender as PictureBox;
            if (clickedRadioButton != null)
            {
                // List of all radio button controls
                var radioButtons = new List<PictureBox>
                {
                    optionOneRadio,
                    optionTwoRadio,
                    optionThreeRadio,
                    optionFourRadio,
                    optionFiveRadio,
                    optionSixRadio
                };

                // Set all other radio buttons to not selected first
                foreach (var radioButton in radioButtons)
                {
                    if (radioButton != clickedRadioButton)
                    {
                        radioButton.Image = Properties.Resources.RadioNotSelected;
                        radioButton.Tag = "not_selected";
                    }
                }

                // Now, set the clicked radio button to selected
                clickedRadioButton.Image = Properties.Resources.RadioSelected;
                clickedRadioButton.Tag = "selected";
            }
        }


        // Event handlers for button hover effects
        private void minimiseButton_MouseEnter(object sender, EventArgs e)
        {
            minimiseButton.Image = Properties.Resources.MinimiseHover;
        }
        private void minimiseButton_MouseLeave(object sender, EventArgs e)
        {
            minimiseButton.Image = Properties.Resources.Minimise;
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            closeButton.Image = Properties.Resources.CloseHover;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            closeButton.Image = Properties.Resources.Close;
        }

        private void copyButton_MouseEnter(object sender, EventArgs e)
        {
            copyButton.Image = Properties.Resources.CopyHover;
        }

        private void copyButton_MouseLeave(object sender, EventArgs e)
        {
            copyButton.Image = Properties.Resources.Copy;
        }

        private void startButton_MouseEnter(object sender, EventArgs e)
        {
            startButton.Image = Properties.Resources.StartHover;
        }

        private void startButton_MouseLeave(object sender, EventArgs e)
        {
            startButton.Image = Properties.Resources.Start;
        }

        private void optionButton_MouseEnter(object sender, EventArgs e)
        {
            PictureBox optionButton = sender as PictureBox;
            if (optionButton != null && optionButton.Tag as string == "not_selected")
            {
                optionButton.Image = Properties.Resources.CheckboxHover;
            }
            else if (optionButton != null && optionButton.Tag as string == "selected")
            {
                optionButton.Image = Properties.Resources.CheckboxSelectedHover;
            }
        }

        private void optionButton_MouseLeave(object sender, EventArgs e)
        {
            PictureBox optionButton = sender as PictureBox;
            if (optionButton != null && optionButton.Tag as string == "not_selected")
            {
                optionButton.Image = Properties.Resources.CheckboxNotSelected;
            }
            else if (optionButton != null && optionButton.Tag as string == "selected")
            {
                optionButton.Image = Properties.Resources.CheckboxSelected;
            }
        }

        private void toggleOptionsButton_MouseEnter(object sender, EventArgs e)
        {
            if (toggleOptionsButton.Tag as string == "toggle_off")
            {
                toggleOptionsButton.Image = Properties.Resources.CheckboxHover;
            }
            else if (toggleOptionsButton.Tag as string == "toggle_on")
            {
                toggleOptionsButton.Image = Properties.Resources.CheckboxSelectedHover;
            }
        }

        private void toggleOptionsButton_MouseLeave(object sender, EventArgs e)
        {
            if (toggleOptionsButton.Tag as string == "toggle_off")
            {
                toggleOptionsButton.Image = Properties.Resources.CheckboxNotSelected;
            }
            else if (toggleOptionsButton.Tag as string == "toggle_on")
            {
                toggleOptionsButton.Image = Properties.Resources.CheckboxSelected;
            }
        }

        private void radioButton_MouseEnter(object sender, EventArgs e)
        {
            PictureBox radioButton = sender as PictureBox;
            if (radioButton != null && radioButton.Tag as string == "not_selected")
            {
                radioButton.Image = Properties.Resources.RadioNotSelectedHover;
            }
            else if (radioButton != null && radioButton.Tag as string == "selected")
            {
                radioButton.Image = Properties.Resources.RadioSelectedHover;
            }
        }

        private void radioButton_MouseLeave(object sender, EventArgs e)
        {
            PictureBox radioButton = sender as PictureBox;
            if (radioButton != null && radioButton.Tag as string == "not_selected")
            {
                radioButton.Image = Properties.Resources.RadioNotSelected;
            }
            else if (radioButton != null && radioButton.Tag as string == "selected")
            {
                radioButton.Image = Properties.Resources.RadioSelected;
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Start button clicked!");
        }

        private Timer clipboardTimer;
        private const int clipboardVisibleDuration = 1500; // Duration for visibility in milliseconds

        private void copyButton_Click(object sender, EventArgs e)
        {
            // Copy the text from textOutput to the clipboard
            Clipboard.SetText(textOutput.Text);

            // Make clipboardNotif visible and start the zoom-in effect
            clipboardNotif.Visible = true;
            isZoomingIn = true;
            isZoomingOut = false;
            clipboardNotif.Size = new Size((int)(clipboardNotif.Width * 0.01), (int)(clipboardNotif.Height * 0.01));
            zoomTimer.Start();

            // Restart the clipboard timer
            clipboardTimer.Stop();
            clipboardTimer.Start();
        }

        private void ZoomTimer_Tick(object sender, EventArgs e)
        {
            if (isZoomingIn)
            {
                // Zoom in effect
                clipboardNotif.Size = new Size((int)(clipboardNotif.Width * (1 + zoomIncrement)), (int)(clipboardNotif.Height * (1 + zoomIncrement)));
                if (clipboardNotif.Width >= this.Width)
                {
                    isZoomingIn = false;
                    zoomTimer.Stop();
                }
            }
            else if (isZoomingOut)
            {
                // Zoom out effect
                clipboardNotif.Size = new Size((int)(clipboardNotif.Width * (1 - zoomIncrement)), (int)(clipboardNotif.Height * (1 - zoomIncrement)));
                if (clipboardNotif.Width <= this.Width * 0.01)
                {
                    isZoomingOut = false;
                    clipboardNotif.Visible = false;
                    zoomTimer.Stop();
                }
            }
        }

        private void ClipboardTimer_Tick(object sender, EventArgs e)
        {
            // Start the zoom-out effect
            isZoomingOut = true;
            isZoomingIn = false;
            zoomTimer.Start();
            clipboardTimer.Stop();
        }

    }
}
