
#region Using Statements
using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if MONODROID
using OpenTK.Graphics.ES11;
#endif
using ShapeManager = FlatRedBall.Math.Geometry.ShapeManager;
using FlatRedBall.Math;
using FlatRedBall.Graphics;
using FlatRedBall.Utilities;
#if !XBOX360 && !XNA4 && !MONOGAME
using FlatRedBall.Graphics.HardwareInstancing;
#endif

#if !WINDOWS_PHONE && !MONOGAME && !XNA4
using PostProcessingManager = FlatRedBall.Graphics.PostProcessing.PostProcessingManager;
#endif

#if WINDOWS_PHONE || MONOGAME
using Effect = FlatRedBall.Graphics.GenericEffect;
#else
using Effect = Microsoft.Xna.Framework.Graphics.Effect;
#endif


using Microsoft.Xna.Framework.Content;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Performance.Measurement;

#if WINDOWS_8
using Windows.System.Threading;
#else

#endif

#endregion

namespace FlatRedBall.Graphics
{
    #region FillVertexLogic class

    class FillVertexLogic
    {
        public IList<Sprite> SpriteList;
        public List<VertexPositionColorTexture[]> VertexLists;
        public int StartIndex;
        public int Count;
        ManualResetEvent mManualResetEvent;
        public int FirstSpriteInAllSimultaneousLogics;

        public FillVertexLogic()
        {
            mManualResetEvent = new ManualResetEvent(false);
        }

        public void Reset()
        {
            mManualResetEvent.Reset();
        }

        public void Wait()
        {
            mManualResetEvent.WaitOne();
        }
        public void FillVertexList()
        {
            Reset();
#if WINDOWS_8
            ThreadPool.RunAsync(FillVertexListInternal);
#else
            ThreadPool.QueueUserWorkItem(FillVertexListInternal);
#endif
        }

        void FillVertexListInternal(object notUsed)
        {
            int vertNum = 0;
            int vertexBufferNum = 0;


            VertexPositionColorTexture[] arrayAtIndex = VertexLists[vertexBufferNum];

            int lastIndexExclusive = StartIndex + Count;


            for (int unadjustedI = StartIndex; unadjustedI < lastIndexExclusive; unadjustedI++)
            {
                int i = unadjustedI - FirstSpriteInAllSimultaneousLogics;

                vertNum = (i * 6) % 6000;
                vertexBufferNum = i / 1000;
                arrayAtIndex = VertexLists[vertexBufferNum];

                Sprite spriteAtIndex = SpriteList[unadjustedI];

                #region the Sprite doesn't have stored vertices (default) so we have to create them now
                if (spriteAtIndex.mAutomaticallyUpdated)
                {
                    // Eventually we'll want to use the actual camera being rendered so 
                    // we can get billboarding to work properly on split screen view.
                    spriteAtIndex.UpdateVertices(SpriteManager.Camera);


                    #region Set the color


#if WINDOWS_8 || IOS
						
						// If the Sprite's Texture is null, it will behave as if it's got its ColorOperation set to Color instead of Texture
					if (spriteAtIndex.ColorOperation == FlatRedBall.Graphics.ColorOperation.Texture && spriteAtIndex.Texture != null)
					{
						// If we are using the texture color, we want to ignore the Sprite's RGB values.  The W component is Alpha, so 
						// we'll use full values for the others.
						//arrayAtIndex[vertNum + 0].Color.PackedValue =
						//    ((uint)(255)) +
						//    (((uint)(255)) << 8) +
						//    (((uint)(255)) << 16) +
						//    (((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) << 24);
						arrayAtIndex[vertNum + 0].Color.PackedValue =
							((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) +
								(((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) << 8) +
								(((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) << 16) +
								(((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) << 24);
					}
					else
					{
						// If we are using the texture color, we 
						arrayAtIndex[vertNum + 0].Color.PackedValue =
							((uint)(255 * spriteAtIndex.mVertices[3].Color.X)) +
								(((uint)(255 * spriteAtIndex.mVertices[3].Color.Y)) << 8) +
								(((uint)(255 * spriteAtIndex.mVertices[3].Color.Z)) << 16) +
								(((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) << 24);
					}
					arrayAtIndex[vertNum + 1].Color.PackedValue =
						arrayAtIndex[vertNum + 0].Color.PackedValue;
					
					arrayAtIndex[vertNum + 2].Color.PackedValue =
						arrayAtIndex[vertNum + 0].Color.PackedValue;
					
					arrayAtIndex[vertNum + 5].Color.PackedValue =
						arrayAtIndex[vertNum + 0].Color.PackedValue;





#elif XNA4

                    arrayAtIndex[vertNum + 0].Color.PackedValue =
                        ((uint)(255 * spriteAtIndex.mVertices[3].Color.X)) +
                        (((uint)(255 * spriteAtIndex.mVertices[3].Color.Y)) << 8) +
                        (((uint)(255 * spriteAtIndex.mVertices[3].Color.Z)) << 16) +
                        (((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) << 24);


                    arrayAtIndex[vertNum + 1].Color.PackedValue =
                        ((uint)(255 * spriteAtIndex.mVertices[0].Color.X)) +
                        (((uint)(255 * spriteAtIndex.mVertices[0].Color.Y)) << 8) +
                        (((uint)(255 * spriteAtIndex.mVertices[0].Color.Z)) << 16) +
                        (((uint)(255 * spriteAtIndex.mVertices[0].Color.W)) << 24);

                    arrayAtIndex[vertNum + 2].Color.PackedValue =
                        ((uint)(255 * spriteAtIndex.mVertices[1].Color.X)) +
                        (((uint)(255 * spriteAtIndex.mVertices[1].Color.Y)) << 8) +
                        (((uint)(255 * spriteAtIndex.mVertices[1].Color.Z)) << 16) +
                        (((uint)(255 * spriteAtIndex.mVertices[1].Color.W)) << 24);

                    arrayAtIndex[vertNum + 5].Color.PackedValue =
                        ((uint)(255 * spriteAtIndex.mVertices[2].Color.X)) +
                        (((uint)(255 * spriteAtIndex.mVertices[2].Color.Y)) << 8) +
                        (((uint)(255 * spriteAtIndex.mVertices[2].Color.Z)) << 16) +
                        (((uint)(255 * spriteAtIndex.mVertices[2].Color.W)) << 24);

#else

                    arrayAtIndex[vertNum + 0].Color.PackedValue =
                        ((uint)(255 * spriteAtIndex.mVertices[3].Color.Z)) +
                        (((uint)(255 * spriteAtIndex.mVertices[3].Color.Y)) << 8) +
                        (((uint)(255 * spriteAtIndex.mVertices[3].Color.X)) << 16) +
                        (((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) << 24);
                    arrayAtIndex[vertNum + 1].Color.PackedValue =
                        ((uint)(255 * spriteAtIndex.mVertices[0].Color.Z)) +
                        (((uint)(255 * spriteAtIndex.mVertices[0].Color.Y)) << 8) +
                        (((uint)(255 * spriteAtIndex.mVertices[0].Color.X)) << 16) +
                        (((uint)(255 * spriteAtIndex.mVertices[0].Color.W)) << 24);
                    arrayAtIndex[vertNum + 2].Color.PackedValue =
                        ((uint)(255 * spriteAtIndex.mVertices[1].Color.Z)) +
                        (((uint)(255 * spriteAtIndex.mVertices[1].Color.Y)) << 8) +
                        (((uint)(255 * spriteAtIndex.mVertices[1].Color.X)) << 16) +
                        (((uint)(255 * spriteAtIndex.mVertices[1].Color.W)) << 24);
                    arrayAtIndex[vertNum + 5].Color.PackedValue =
                        ((uint)(255 * spriteAtIndex.mVertices[2].Color.Z)) +
                        (((uint)(255 * spriteAtIndex.mVertices[2].Color.Y)) << 8) +
                        (((uint)(255 * spriteAtIndex.mVertices[2].Color.X)) << 16) +
                        (((uint)(255 * spriteAtIndex.mVertices[2].Color.W)) << 24);
#endif
                    #endregion


                    arrayAtIndex[vertNum + 0].Position = spriteAtIndex.mVertices[3].Position;
                    arrayAtIndex[vertNum + 0].TextureCoordinate = spriteAtIndex.mVertices[3].TextureCoordinate;


                    arrayAtIndex[vertNum + 1].Position = spriteAtIndex.mVertices[0].Position;
                    arrayAtIndex[vertNum + 1].TextureCoordinate = spriteAtIndex.mVertices[0].TextureCoordinate;

                    arrayAtIndex[vertNum + 2].Position = spriteAtIndex.mVertices[1].Position;
                    arrayAtIndex[vertNum + 2].TextureCoordinate = spriteAtIndex.mVertices[1].TextureCoordinate;

                    arrayAtIndex[vertNum + 3] = arrayAtIndex[vertNum + 0];
                    arrayAtIndex[vertNum + 4] = arrayAtIndex[vertNum + 2];

                    arrayAtIndex[vertNum + 5].Position = spriteAtIndex.mVertices[2].Position;
                    arrayAtIndex[vertNum + 5].TextureCoordinate = spriteAtIndex.mVertices[2].TextureCoordinate;

                    if (spriteAtIndex.FlipHorizontal)
                    {
                        arrayAtIndex[vertNum + 0].TextureCoordinate = arrayAtIndex[vertNum + 5].TextureCoordinate;
                        arrayAtIndex[vertNum + 5].TextureCoordinate = arrayAtIndex[vertNum + 3].TextureCoordinate;
                        arrayAtIndex[vertNum + 3].TextureCoordinate = arrayAtIndex[vertNum + 0].TextureCoordinate;

                        arrayAtIndex[vertNum + 2].TextureCoordinate = arrayAtIndex[vertNum + 1].TextureCoordinate;
                        arrayAtIndex[vertNum + 1].TextureCoordinate = arrayAtIndex[vertNum + 4].TextureCoordinate;
                        arrayAtIndex[vertNum + 4].TextureCoordinate = arrayAtIndex[vertNum + 2].TextureCoordinate;
                    }
                    if (spriteAtIndex.FlipVertical)
                    {
                        arrayAtIndex[vertNum + 0].TextureCoordinate = arrayAtIndex[vertNum + 1].TextureCoordinate;
                        arrayAtIndex[vertNum + 1].TextureCoordinate = arrayAtIndex[vertNum + 3].TextureCoordinate;
                        arrayAtIndex[vertNum + 3].TextureCoordinate = arrayAtIndex[vertNum + 0].TextureCoordinate;

                        arrayAtIndex[vertNum + 2].TextureCoordinate = arrayAtIndex[vertNum + 5].TextureCoordinate;
                        arrayAtIndex[vertNum + 5].TextureCoordinate = arrayAtIndex[vertNum + 4].TextureCoordinate;
                        arrayAtIndex[vertNum + 4].TextureCoordinate = arrayAtIndex[vertNum + 2].TextureCoordinate;
                    }
                }
                #endregion
                else
                {
                    arrayAtIndex[vertNum + 0] = spriteAtIndex.mVerticesForDrawing[3];
                    arrayAtIndex[vertNum + 1] = spriteAtIndex.mVerticesForDrawing[0];
                    arrayAtIndex[vertNum + 2] = spriteAtIndex.mVerticesForDrawing[1];
                    arrayAtIndex[vertNum + 3] = spriteAtIndex.mVerticesForDrawing[3];
                    arrayAtIndex[vertNum + 4] = spriteAtIndex.mVerticesForDrawing[1];
                    arrayAtIndex[vertNum + 5] = spriteAtIndex.mVerticesForDrawing[2];
                }

            }
            mManualResetEvent.Set();

        }
    }

    #endregion

    #region XML Docs
    /// <summary>
    /// Static class responsible for drawing/rendering content to the cameras on screen.
    /// </summary> 
    /// <remarks>This class is called by <see cref="FlatRedBallServices.Draw()"/></remarks>
    #endregion
    public static partial class Renderer
    {
        #region Enums
        static int EffectParameterNamesCount = 24;
        enum EffectParameterNamesEnum
        {
            LightingEnable,
            AmbLight0Enable,
            AmbLight0DiffuseColor,
            DirLight0Enable,
            DirLight0Direction,
            DirLight0DiffuseColor,
            DirLight0SpecularColor,
            DirLight1Enable,
            DirLight1Direction,
            DirLight1DiffuseColor,
            DirLight1SpecularColor,
            DirLight2Enable,
            DirLight2Direction,
            DirLight2DiffuseColor,
            DirLight2SpecularColor,
            PointLight0Enable,
            PointLight0Position,
            PointLight0DiffuseColor,
            PointLight0SpecularColor,
            PointLight0Range,
            FogEnabled,
            FogColor,
            FogStart,
            FogEnd
        }
        #endregion

        #region Fields

        static IGraphicsDeviceService mGraphics;
        public static SpriteBatch mSpriteBatch;
        static List<FillVertexLogic> mFillVertexLogics = new List<FillVertexLogic>();

        #region Render Targets and textures

        public static Dictionary<int, SurfaceFormat> RenderModeFormats;



        static RenderMode mCurrentRenderMode = RenderMode.Default;

        #endregion

        #region Vertex Fields

        // Vertex buffers
        static List<DynamicVertexBuffer> mVertexBufferList;
        static List<DynamicVertexBuffer> mShapesVertexBufferList;
        //static DynamicVertexBuffer vertexBuffer;
        // static int mNumberOfElementsInLastBuffer;

        static List<VertexPositionColorTexture[]> mSpriteVertices = new List<VertexPositionColorTexture[]>();
        static List<VertexPositionColorTexture[]> mZBufferedSpriteVertices = new List<VertexPositionColorTexture[]>();
        static List<VertexPositionColorTexture[]> mTextVertices = new List<VertexPositionColorTexture[]>();
        static List<VertexPositionColor[]> mShapeVertices = new List<VertexPositionColor[]>();

        // Vertex declarations
        static VertexDeclaration mPositionColorTexture;
        static VertexDeclaration mPositionColor;

        // Vertex arrays
        static VertexPositionColorTexture[] mVertexArray;
        static VertexPositionColor[] mShapeDrawingVertexArray;

        // Render breaks
        static List<RenderBreak> mRenderBreaks = new List<RenderBreak>();
        static List<RenderBreak> mSpriteRenderBreaks = new List<RenderBreak>();
        static List<RenderBreak> mZBufferedSpriteRenderBreaks = new List<RenderBreak>();
        static List<RenderBreak> mTextRenderBreaks = new List<RenderBreak>();

        // Current Vertex buffer
        static VertexBuffer mVertexBuffer;
        static IndexBuffer mIndexBuffer;

        #endregion

        #region Effects

#if !WINDOWS_PHONE && !MONOGAME
        static BasicEffect mBasicEffect;
        static Effect mEffect;
        static Effect mCurrentEffect;
        
#else
        
        static GenericEffect mGenericEffect;
        static GenericEffect mEffect = new GenericEffect( GenericEffect.DefaultShaderType.Determine );
        static GenericEffect mAlphaTestEffect;
        static GenericEffect mCurrentEffect = new GenericEffect( GenericEffect.DefaultShaderType.Determine );
#endif

#if !WINDOWS_PHONE && !MONOGAME

#if !XNA4
        static internal EffectPool mEffectPool = new EffectPool();
#endif

        static EffectCache mModelEffectParameterCache;

        public static Dictionary<int, String> EffectTechniqueNames;
#endif

        #endregion

        #region Texture Fields

        static Texture2D mTexture;
        static string mColorOperation;
        static BlendOperation mBlendOperation;
        static TextureAddressMode mTextureAddressMode;

        #region Xml Docs
        // When setting the ColorOperation using the ColorOperation enum
        // the enum has to be converted to a string.  This allocates TONS of
        // memory.  To reduce this we'll just cache off the last value set as well
        // and simply compare against that to prevent unnecessary StringEnum.GetStringValue
        // calls.
        #endregion
        static internal ColorOperation mLastColorOperationSet = ColorOperation.None;

        #endregion

        #region Debugging Information

        internal static int NumberOfSpritesDrawn;
        static int mFillVBListCallsThisFrame;
        static int mRenderBreaksAllocatedThisFrame;

        #endregion

        public static string[] EffectParameterNames;

        static string[] DirLightEnable = new string[16];
        static string[] DirLightDirection = new string[16];
        static string[] DirLightDiffuseColor = new string[16];
        static string[] DirLightSpecularColor = new string[16];

        static VertexPositionTexture[] mQuadVertices;
        static short[] mQuadIndices;
        static VertexDeclaration mQuadVertexDeclaration;

        // Shape models / effects
#if !XNA4
        //static Microsoft.Xna.Framework.Graphics.Model mSphereShape;
        //static Microsoft.Xna.Framework.Graphics.Model mCubeShape;
#endif
        static BasicEffect mWireframeEffect;

        #endregion

        #region Properties

        #region Public Properties

#if !XNA4 && !MONOGAME
        #region XML Docs
        /// <summary>
        /// Returns the maximum supported vertex shader profile on the user's device
        /// </summary>
        #endregion
        public static ShaderProfile VertexShaderProfile
        {
            get { return GraphicsDevice.GraphicsDeviceCapabilities.MaxVertexShaderProfile; }
        }


        #region XML Docs
        /// <summary>
        /// Returns the maximum supported pixel shader profile on the user's device
        /// </summary>
        #endregion
        public static ShaderProfile PixelShaderProfile
        {
            get { return GraphicsDevice.GraphicsDeviceCapabilities.MaxPixelShaderProfile; }
        }
#endif

        //public static RendererDiagnosticSettings RendererDiagnosticSettings
        //{
        //    get;
        //    set;
        //}




        static public Texture2D Texture
        {
            set
            {
                if (value != mTexture )
                {

                    ForceSetTexture(value);
                }
                //else
                //{
                //    mTexture = value;
                //}
            }
        }

        private static void ForceSetTexture(Texture2D value)
        {
            mTexture = value;
#if WINDOWS_PHONE || MONOGAME || IOS
            mCurrentEffect.TextureEnabled = value != null;
            mCurrentEffect.Texture = mTexture;
#else
                    mCurrentEffect.Parameters["CurrentTexture"].SetValue(mTexture);
#endif
        }

        static public Texture2D TextureOnDevice
        {
            set
            {
                if( value != mTexture)
                {
                    mTexture = value;
                    GraphicsDevice.Textures[ 0 ] = mTexture;
                }
            }
        }

        static public VertexBuffer VertexBuffer
        {
            set
            {
                if( value != mVertexBuffer && value != null )
                {
                    mVertexBuffer = value;
#if XNA4

                    GraphicsDevice.SetVertexBuffer( mVertexBuffer );
#else
                    throw new NotImplementedException();
#endif
                }
                else
                {
                    mVertexBuffer = null;
                }
            }
        }

        static public IndexBuffer IndexBuffer
        {
            set
            {
                if( value != mIndexBuffer && value != null )
                {
                    mIndexBuffer = value;
                    GraphicsDevice.Indices = mIndexBuffer;
                }
                else
                {
                    mIndexBuffer = null;
                }
            }
        }

        /// <summary>
        /// Returns the Layer currently being rendered.  Can be used in
        /// IDrawableBatches and debug code.
        /// </summary>
        public static Layer CurrentLayer
        {
            get;
            private set;
        }

        internal static string CurrentLayerName
        {
            get
            {
                if (CurrentLayer != null)
                {
                    return CurrentLayer.Name;
                }
                else
                {
                    return "Unlayered";
                }
            }
        }

        static public void SetCurrentEffect(Effect value, Camera camera)
        {
#if !WINDOWS_PHONE && !MONOGAME

            mCurrentEffect = value; 
            //internal get { return mCurrentEffect; }
#else
                if (value != mCurrentEffect)
                {
                    Texture2D oldTexture = null;
                    if (mCurrentEffect != null)
                    {
                        oldTexture = mCurrentEffect.Texture;
                    }

                    mCurrentEffect = value;
                    if (mCurrentEffect.LightingEnabled)
                    {
                        mCurrentEffect.LightingEnabled = false;
                    }
                    camera.SetDeviceViewAndProjection(mCurrentEffect, false);

                    ForceSetTexture(oldTexture);
                }
#endif
        }
         

        static public VertexDeclaration PositionColorVertexDeclaration
        {
            get { return mPositionColor; }
        }

        public static VertexDeclaration PositionColorTextureVertexDeclaration
        {
            get { return mPositionColorTexture; }
        }





        public static bool IsInRendering
        {
            get;
            set;
        }

        public static RenderMode CurrentRenderMode
        {
            get { return mCurrentRenderMode; }
        }

        public static int RenderBreaksAllocatedThisFrame
        {
            get
            {
                return mRenderBreaksAllocatedThisFrame;
            }
        }

        static bool mRecordRenderBreaks;
        public static bool RecordRenderBreaks
        {
            get
            {
                return mRecordRenderBreaks;
            }
            set
            {
                mRecordRenderBreaks = value;
                if (mRecordRenderBreaks && LastFrameRenderBreakList == null)
                {
                    LastFrameRenderBreakList = new List<RenderBreak>();
                }
                if(!mRecordRenderBreaks && LastFrameRenderBreakList != null)
                {
                    LastFrameRenderBreakList.Clear();
                }
            }
        }

        public static List<RenderBreak> LastFrameRenderBreakList
        {
            get;
            private set;
        }


        #endregion

        #region Internal Properties
        

        static internal ColorOperation ColorOperation
        {
            get
            {
                return mLastColorOperationSet;
            }
            set
            {
                if (mLastColorOperationSet != value)
                {
                    ForceSetColorOperation(value);
                }
            }
        }

        /// <summary>
        /// Sets the blend operation on the graphics device if the set value differs from the current value.
        /// If the two values are the same, then the property doesn't do anything.
        /// </summary>
        public static BlendOperation BlendOperation
        {
            get
            {
                return mBlendOperation;
            }
            set
            {
                if (value != mBlendOperation)
                {
                    mBlendOperation = value;

                    ForceSetBlendOperation();
                }
            }
        }

        public static TextureAddressMode TextureAddressMode
        {
            get
            {
                return mTextureAddressMode;
            }
            set
            {
                if (value != mTextureAddressMode)
                {
                    ForceSetTextureAddressMode(value);
                }
            }
        }

        public static void ForceSetTextureAddressMode(Microsoft.Xna.Framework.Graphics.TextureAddressMode value)
        {
            mTextureAddressMode = value;

#if WINDOWS_PHONE || MONOGAME
            mCurrentEffect.SetTextureAddressModeNoCall(mTextureAddressMode);
#endif

#if XNA4
            FlatRedBallServices.GraphicsOptions.ForceRefreshSamplerState(0);
            FlatRedBallServices.GraphicsOptions.ForceRefreshSamplerState(1);

#elif XBOX_360
                    mCurrentEffect.Parameters["Address"].SetValue(((int)mTextureAddressMode) - 1);
#else
                    mCurrentEffect.Parameters["Address"].SetValue((int)mTextureAddressMode);
#endif
        }

        static internal IGraphicsDeviceService Graphics
        {
            get { return mGraphics; }
        }

        static internal GraphicsDevice GraphicsDevice
        {
            get 
			{ 
				return mGraphics.GraphicsDevice; 
			}
        }

        static public Effect Effect
        {
            set 
            { 
                mEffect = value; 
            }
            get { return mEffect; }
        }

        #endregion

        #endregion

        #region Methods

        #region Constructor and Initialize



        static void InitializeEffectParameterStrings()
        {
            EffectParameterNames = new string[EffectParameterNamesCount];
            for (int i = 0; i < EffectParameterNamesCount; i++)
            {
                EffectParameterNames[i] = ((EffectParameterNamesEnum)i).ToString();
            }
        }

        static Renderer()
        {
            //RendererDiagnosticSettings = new Performance.Measurement.RendererDiagnosticSettings();

            // Vertex Buffers
            mVertexBufferList = new List<DynamicVertexBuffer>();
            mShapesVertexBufferList = new List<DynamicVertexBuffer>();

            // Vertex Arrays
            mVertexArray = new VertexPositionColorTexture[6000];
            mShapeDrawingVertexArray = new VertexPositionColor[6000];
            

            #region pre-create strings that will greatly reduce memory allocation

            InitializeEffectParameterStrings();

            for (int i = 0; i < DirLightEnable.Length; i++)
            {
                DirLightEnable[i] = String.Format("DirLight{0}Enable", i);
            }

            for (int i = 0; i < DirLightDirection.Length; i++)
            {
                DirLightDirection[i] = String.Format("DirLight{0}Direction", i);
            }

            for (int i = 0; i < DirLightDiffuseColor.Length; i++)
            {
                DirLightDiffuseColor[i] = String.Format("DirLight{0}DiffuseColor", i);
            }

            for (int i = 0; i < DirLightSpecularColor.Length; i++)
            {
                DirLightSpecularColor[i] = String.Format("DirLight{0}SpecularColor", i);
            }

            #endregion
            SetNumberOfThreadsToUse(1);
        }

        internal static void Initialize(IGraphicsDeviceService graphics)
        {
            // Make sure the device isn't null
            if (graphics.GraphicsDevice == null)
            {
                throw new NullReferenceException("The GraphicsDevice is null.  Are you calling FlatRedBallServices.InitializeFlatRedBall from the Game's constructor?  If so, you need to call it in the Initialize or LoadGraphicsContent method.");
            }

            mGraphics = graphics;

#if MONOGAME || WINDOWS_PHONE
            mAlphaTestEffect = new GenericEffect(GenericEffect.DefaultShaderType.AlphaTest);
#endif


            InitializeEffect();

#if XNA4
            ForceSetBlendOperation();
#else
            // This needs to be done outside of the property so that it matches the default "Regular" setting:
            mGraphics.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            mGraphics.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
#endif
        }


        private static void InitializeEffect()
        {


#if XNA4
            mPositionColorTexture = VertexPositionColorTexture.VertexDeclaration;
            mPositionColor = VertexPositionColor.VertexDeclaration;
#else
            // Vertex Declarations
            mPositionColorTexture = new VertexDeclaration(
                mGraphics.GraphicsDevice, VertexPositionColorTexture.VertexElements);
            mPositionColor = new VertexDeclaration(
                mGraphics.GraphicsDevice, VertexPositionColor.VertexElements);
#endif



            #region Verify available render modes

            // Create render mode formats dictionary
            RenderModeFormats = new Dictionary<int, SurfaceFormat>(10);
            RenderModeFormats.Add((int)RenderMode.Color, SurfaceFormat.Color);
            RenderModeFormats.Add((int)RenderMode.Default, SurfaceFormat.Color);
#if !XNA4


            if (GraphicsDevice.CreationParameters.Adapter.CheckDeviceFormat(
                GraphicsDevice.CreationParameters.DeviceType,
                GraphicsDevice.DisplayMode.Format,
                TextureUsage.None, QueryUsages.None,
                ResourceType.RenderTarget,
                SurfaceFormat.Single))
            {
                RenderModeFormats.Add((int)RenderMode.Depth, SurfaceFormat.Single);   
            }
            if (GraphicsDevice.CreationParameters.Adapter.CheckDeviceFormat(
                GraphicsDevice.CreationParameters.DeviceType,
                GraphicsDevice.DisplayMode.Format,
                TextureUsage.None, QueryUsages.None,
                ResourceType.RenderTarget,
                SurfaceFormat.HalfVector4))
            {
                RenderModeFormats.Add((int)RenderMode.Normals, SurfaceFormat.HalfVector4);
                RenderModeFormats.Add((int)RenderMode.Position, SurfaceFormat.Color);
            }
            else if (GraphicsDevice.CreationParameters.Adapter.CheckDeviceFormat(
                GraphicsDevice.CreationParameters.DeviceType,
                GraphicsDevice.DisplayMode.Format,
                TextureUsage.None, QueryUsages.None,
                ResourceType.RenderTarget,
                SurfaceFormat.Vector4))
            {
                RenderModeFormats.Add((int)RenderMode.Normals, SurfaceFormat.Vector4);
                RenderModeFormats.Add((int)RenderMode.Position, SurfaceFormat.Vector4);
            }
#else

#endif

            #endregion


#if !WINDOWS_PHONE && !MONOGAME
            // Create render mode technique names dictionary
            EffectTechniqueNames = new Dictionary<int, String>();
            EffectTechniqueNames.Add((int)RenderMode.Default, "RenderDefault");
            EffectTechniqueNames.Add((int)RenderMode.Color, "RenderColor");
            if (RenderModeFormats.ContainsKey((int)RenderMode.Depth))
            {
                EffectTechniqueNames.Add((int)RenderMode.Depth, "RenderDepth");
            }
            if (RenderModeFormats.ContainsKey((int)RenderMode.Normals))
            {
                EffectTechniqueNames.Add((int)RenderMode.Normals, "RenderNormals");
                EffectTechniqueNames.Add((int)RenderMode.Position, "RenderPosition");
            }
#endif
            //// Create render target pairs
            //for (int i = 0; i < RenderModeFormats.Keys.Count; i++)
            //{
            //    InitializeRenderTargets(RenderModeFormats[(RenderMode)i]);
            //}

            // Set the initial viewport

            Viewport viewport = mGraphics.GraphicsDevice.Viewport;
            viewport.Width = FlatRedBallServices.ClientWidth;
            viewport.Height = FlatRedBallServices.ClientHeight;
            mGraphics.GraphicsDevice.Viewport = viewport;

            // Sprite batch
            mSpriteBatch = new SpriteBatch(FlatRedBallServices.GraphicsDevice);

            // Basic effect

#if WINDOWS_PHONE || MONOGAME
            mGenericEffect = new GenericEffect( GenericEffect.DefaultShaderType.Basic );
            mGenericEffect.Alpha = 1.0f;
            mGenericEffect.AmbientLightColor = new Vector3(1f, 1f, 1f);
            mGenericEffect.World = Matrix.Identity;
#elif XNA4
            mBasicEffect = new BasicEffect(mGraphics.GraphicsDevice);
            mBasicEffect.Alpha = 1.0f;
            mBasicEffect.AmbientLightColor = new Vector3(1f, 1f, 1f);
            mBasicEffect.World = Matrix.Identity;
#else
            mBasicEffect = new BasicEffect(mGraphics.GraphicsDevice, null);
            mBasicEffect.Alpha = 1.0f;
            mBasicEffect.AmbientLightColor = new Vector3(1f, 1f, 1f);
            mBasicEffect.World = Matrix.Identity;
            mBasicEffect.CommitChanges();
#endif

#if XNA4 || MONOGAME
            // TODO:  Add the resources or the test sphere and cube xnb files so that they can be used.

#else
            // Shapes
            //mSphereShape =
            //    FlatRedBallServices.mResourceContentManager.Load<Microsoft.Xna.Framework.Graphics.Model>("test_sphere");
            //mCubeShape =
            //    FlatRedBallServices.mResourceContentManager.Load<Microsoft.Xna.Framework.Graphics.Model>("test_cube");
#endif

#if XNA4 || MONOGAME
            mWireframeEffect = new BasicEffect(FlatRedBallServices.GraphicsDevice);
#elif WINDOWS_PHONE
            mWireframeEffect = new 
#else
            mWireframeEffect = new BasicEffect(FlatRedBallServices.GraphicsDevice, null);
#endif

#if XNA4 || MONOGAME
            // TODO:  Since these aren't loaded yet I'm not messing with the effect yet
#else
            //// Replace all shape effects
            //foreach (ModelMesh mesh in mSphereShape.Meshes)
            //{
            //    foreach (ModelMeshPart part in mesh.MeshParts)
            //    {
            //        part.Effect = mWireframeEffect;
            //    }
            //}

            //foreach (ModelMesh mesh in mCubeShape.Meshes)
            //{
            //    foreach (ModelMeshPart part in mesh.MeshParts)
            //    {
            //        part.Effect = mWireframeEffect;
            //    }
            //}

#endif

            // Set up device settings


#if XNA4 || MONOGAME
            BlendOperation = FlatRedBall.Graphics.BlendOperation.Regular;


            DepthStencilState depthStencilState = new DepthStencilState();
            depthStencilState.DepthBufferEnable = false;
            depthStencilState.DepthBufferWriteEnable = false;

			mGraphics.GraphicsDevice.DepthStencilState = depthStencilState;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            


#else
            mGraphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;

            BlendOperation = BlendOperation.Regular;
            //mGraphics.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            //mGraphics.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            // Vic says - we WERE at one time setting the blends like this, but this
            // causes problems when ONLY rendering objects with a non-regular blend.  So instead
            // we use the BlendOperation property.
            //mGraphics.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            //mGraphics.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            mGraphics.GraphicsDevice.RenderState.DepthBufferEnable = false;
            mGraphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;

            mGraphics.GraphicsDevice.RenderState.AlphaTestEnable = true;
            mGraphics.GraphicsDevice.RenderState.ReferenceAlpha = 0;
            mGraphics.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Greater;
            mGraphics.GraphicsDevice.RenderState.CullMode = CullMode.None;



#endif
        }

        #region XML Docs
        /// <summary>
        /// Ensures that a render target pair of this surface format exists
        /// </summary>
        /// <param name="surfaceFormat">The surface format for the render targets</param>
        #endregion
        internal static void InitializeRenderTargets(SurfaceFormat surfaceFormat)
        {

            // Get the largest drawn camera size
            int screenwidth = FlatRedBallServices.mGraphics.PreferredBackBufferWidth;
            int screenheight = FlatRedBallServices.mGraphics.PreferredBackBufferHeight;

            //foreach (Camera camera in Cameras)
            //{
            //    if (camera.Width > screenwidth)
            //    {
            //        screenwidth = camera.Width;
            //    }
            //    if (camera.Height > screenheight)
            //    {
            //        screenheight = camera.Height;
            //    }
            //}

            // Make sure the current render target (if it exists) is large enough, and/or create
            // a large enough render target
            
            // OLD CODE
            //for (int i = 0; i < Cameras.Count; i++)
            //{
            //    if (!Cameras[i].PostProcessing.mRenderTargets.ContainsKey(surfaceFormat))
            //    {
            //        Cameras[i].PostProcessing.mRenderTargets.Add(
            //            surfaceFormat, new RenderTargetPair(
            //                surfaceFormat, Cameras[i].Width, Cameras[i].Height));
            //    }
            //}
        }

        #endregion

        #region Public Methods

        #region Main Drawing Methods

        
        private static void PrepareForDrawScene(Camera camera, RenderMode renderMode)
        {
            mCurrentRenderMode = renderMode;


            #region Set the effect parameters for this camera
             
#if XNA4 || MONOGAME
            // TODO:  Need to investigate shared effect parameters.  Do they exist on Windows Phone?
            // throw new NotImplementedException();
#else
            SetSharedEffectParameters(camera);
            SetSharedEffectParameters(camera, ModelManager.mBasicModelEffect, ModelManager.mBasicModelEffectCache);
            foreach (ContentManager contentManager in FlatRedBallServices.mContentManagers.Values)
            {
                if (contentManager is FlatRedBall.Content.ContentManager)
                {

                    Effect firstEffect = ((FlatRedBall.Content.ContentManager)contentManager).mFirstEffect;

                    if (firstEffect != null && ((FlatRedBall.Content.ContentManager)contentManager).mFirstEffectCache != null)
                    {
                        SetSharedEffectParameters(camera, firstEffect, ((FlatRedBall.Content.ContentManager)contentManager).mFirstEffectCache);
                    }
                }

            }
#endif

            #endregion



            // Set the viewport for the current camera
            Viewport viewport = camera.GetViewport();

            mGraphics.GraphicsDevice.Viewport = viewport;

            #region Clear the viewport

            if (renderMode == RenderMode.Default || renderMode == RenderMode.Color)
            {
                // Vic says:  This code used to be:
                //if (!mUseRenderTargets && camera.BackgroundColor.A == 0)
                // Why prevent color clearing only when we aren't using render targets?  Don't know, 
                // so I changed this in June 

                // UPDATE:
                // It seems that removing the !mUseRenderTargets just makes the background purple...no change
                // happens in tems of things being able to be drawn.  Not sure why, but I'll update the docs to
                // indicate that you can't use RenderTargets and have stuff drawn before FRB


                if (camera.BackgroundColor.A == 0)
                {
                    if (camera.ClearsDepthBuffer)
                    {
                        // clearing to a transparent color, so just clear depth
                        mGraphics.GraphicsDevice.Clear(ClearOptions.DepthBuffer,
                            camera.BackgroundColor, 1, 0);
                    }
                }
                else
                {
                    if (camera.ClearsDepthBuffer)
                    {

                        mGraphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer,
                            camera.BackgroundColor, 1, 0);
                    }
                    else
                    {
                        mGraphics.GraphicsDevice.Clear(ClearOptions.Target,
                            camera.BackgroundColor, 1, 0);
                    }
#if SUPPORTS_POST_PROCESSING
                    
                    if (camera.ClearsTargetDefaultRenderMode)
                    {
                        mGraphics.GraphicsDevice.Clear(ClearOptions.Target,
                            camera.BackgroundColor, 1, 0);
                    }
#endif
                }
            }
            else if (renderMode == RenderMode.Depth)
            {
                if (camera.ClearsDepthBuffer)
                {
                    mGraphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer,
                        Color.White, 1, 0);
                }
            }
            else
            {
                if (camera.ClearsDepthBuffer)
                {
#if XNA4
                    Color colorToClearTo = Color.Transparent;
#else
                    Color colorToClearTo = Color.TransparentBlack;
#endif

                    mGraphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer,
                       colorToClearTo, 1, 0);
                }
            }

            #endregion



            #region Set device settings for rendering

#if XNA4
            // do nothing???

            // Let's force it just in case someone screwed with it outside of the rendering - 
            // like when using render states
            ForceSetTextureAddressMode(Microsoft.Xna.Framework.Graphics.TextureAddressMode.Clamp);

#else
            // Reset alpha blending
            mGraphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;

            BlendOperation = BlendOperation.Regular;

            mGraphics.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = true;
            mGraphics.GraphicsDevice.RenderState.AlphaBlendOperation = BlendFunction.Max;
            mGraphics.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.DestinationAlpha;
            mGraphics.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.SourceAlpha;
            

            // Reset alpha test
            mGraphics.GraphicsDevice.RenderState.AlphaTestEnable = true;
            mGraphics.GraphicsDevice.RenderState.ReferenceAlpha = 0;
            mGraphics.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Greater;
#endif
            #endregion






            #region Set camera values on the current effect
#if WINDOWS_PHONE || MONOGAME
            mCurrentEffect = mEffect;
            if (mEffect.LightingEnabled)
            {
                mEffect.LightingEnabled = false;
            }
            camera.SetDeviceViewAndProjection(mEffect, false);
#else
            mCurrentEffect = mEffect;
            camera.SetDeviceViewAndProjection(mCurrentEffect, false);
#endif
            #endregion
        }

        #endregion

#if !WINDOWS_PHONE && !MONOGAME
        public static Texture2D GetTextureFromEffect(Effect effect)
        {

            if (effect is BasicEffect)
            {
                return ((BasicEffect)effect).Texture;
            }
            if (effect.Parameters["BasicTexture"] != null)
            {
                return effect.Parameters["BasicTexture"].GetValueTexture2D();
            }
            if (effect.Parameters["Texture"] != null)
            {
                return effect.Parameters["Texture"].GetValueTexture2D();
            }
            if (effect.Parameters["DiffuseTexture"] != null)
            {
                return effect.Parameters["DiffuseTexture"].GetValueTexture2D();
            }
            return null;
        }
#else
        public static Texture2D GetTextureFromEffect(Microsoft.Xna.Framework.Graphics.Effect effect)
        {
            GenericEffect effectToReturn = new GenericEffect(effect);
            return effectToReturn.Texture;
        }
#endif

        public static void SetEffectTexture(Microsoft.Xna.Framework.Graphics.Effect effect, Texture2D texture2D)
        {
            if (effect is BasicEffect)
            {
                ((BasicEffect)effect).TextureEnabled = true;
                ((BasicEffect)effect).Texture = texture2D;
            }
            if (effect.Parameters["BasicTexture"] != null)
            {
                effect.Parameters["BasicTexture"].SetValue(texture2D);
            }
            if (effect.Parameters["Texture"] != null)
            {
                effect.Parameters["Texture"].SetValue(texture2D);
            }
            if (effect.Parameters["DiffuseTexture"] != null)
            {
                effect.Parameters["DiffuseTexture"].SetValue(texture2D);
            }
            if (effect.Parameters["TextureEnabled"] != null)
            {
                effect.Parameters["TextureEnabled"].SetValue(true);
            }
        }

        public static new String ToString()
        {
            return String.Format(
                "Number of RenderBreaks allocated: %d\nNumber of Sprites drawn: %d",
                mRenderBreaksAllocatedThisFrame, NumberOfSpritesDrawn);
        }

        #endregion

        #region Internal Methods

        public static void Update()
        {
        }

        internal static void UpdateDependencies()
        {
        }

        public static void Draw()
        {
            Draw(null);
        }

        // made public for those who want 
        // to have more control over how FRB
        // renders.
        public static void Draw(Section section)
        {
            if (section != null)
            {
                Section.GetAndStartContextAndTime("Start of Renderer.Draw");
            }
            
            IsInRendering = true;

            // Drawing should only occur if the window actually has pixels
            //if (FlatRedBallServices.Game.Window.ClientBounds.Height == 0 ||
            //    FlatRedBallServices.Game.Window.ClientBounds.Width == 0)
            // Using ClientBounds causes memory to be allocated. We can just
            // use the FlatRedBallServices' value which gets updated whenever
            // the resolution changes.
            if(FlatRedBallServices.mClientWidth == 0 ||
                FlatRedBallServices.mClientHeight == 0)                
            {
                IsInRendering = false;
                return;
            }

            #region Reset the debugging and profiling information

            mFillVBListCallsThisFrame = 0;
            mRenderBreaksAllocatedThisFrame = 0;
            if (LastFrameRenderBreakList != null)
            {
                LastFrameRenderBreakList.Clear();
            }

            NumberOfSpritesDrawn = 0;


            #endregion



            #region Make sure there is at least one camera
#if DEBUG
            if (SpriteManager.Cameras.Count <= 0)
            {
                NullReferenceException exception = new NullReferenceException(
                    "There are no cameras to render, did you forget to add a camera to the SpriteManager?");
                throw exception;
            }
#endif
            #endregion

            #region Make sure the GraphicsDeviceManager is not null
#if DEBUG
            if (mGraphics == null || mGraphics.GraphicsDevice == null)
            {
                NullReferenceException exception = new NullReferenceException(
                    "Renderer's GraphicsDeviceManager is null.  Did you forget to call FlatRedBallServices.Initialize?");
                throw exception;
            }
#endif
            #endregion


            if (section != null)
            {
                Section.EndContextAndTime();
                Section.GetAndStartContextAndTime("Render Cameras");
            }

            #region Loop through all cameras (viewports)
            // Note: It may be more efficient to do this loop at each point there is
            //       a camera reference to avoid passing geometry multiple times.
            //  Addition: The noted idea may not be faster due to render target swapping.



            for (int i = 0; i < SpriteManager.Cameras.Count; i++)
            {
                // July 25, 2014
                // I don't think we need to do this anymore.  This is old code that
                // doesn't really fit the pattern.  The Camera should be passed in rather
                // than set through this function:
                //Camera camera = SpriteManager.SetCurrentCamera(c);
                Camera camera = SpriteManager.Cameras[i];

                lock (Renderer.Graphics.GraphicsDevice)
                {

                    #region If the Camera either DrawsWorld or DrawsCameraLayer, then perform drawing
                    if (camera.DrawsWorld || camera.DrawsCameraLayer)
                    {
                        if (section != null)
                        {
                            string cameraName = camera.Name;
                            if (string.IsNullOrEmpty(cameraName))
                            {
                                cameraName = "at index " + i;
                            }
                            Section.GetAndStartContextAndTime("Render camera " + cameraName);
                        }

                        DrawCamera(camera, RenderMode.Default, section);

                        if (section != null)
                        {
                            Section.EndContextAndTime();
                        }
                    }
                    #endregion
                }
            }


            #endregion

            if (section != null)
            {
                Section.EndContextAndTime();
                Section.GetAndStartContextAndTime("End of Render");
            }
            

            #region Reset device settings for rendering

#if !XNA4 && !SILVERLIGHT && !WINDOWS_8
            // Reset texture filter
            FlatRedBallServices.GraphicsOptions.ResetTextureFilter();
#endif

#if !XNA4
            // Reset alpha blending
            mGraphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            BlendOperation = BlendOperation.Regular;

            // Reset alpha test
            mGraphics.GraphicsDevice.RenderState.AlphaTestEnable = true;
            mGraphics.GraphicsDevice.RenderState.ReferenceAlpha = 0;
            mGraphics.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Greater;
#endif

            #endregion

            IsInRendering = false;

            Screens.ScreenManager.Draw();

            if (section != null)
            {
                Section.EndContextAndTime();
            }
        }

#if WINDOWS_PHONE || MONOGAME

        internal static void SetFogForColorOperation(float red, float green, float blue)
        {

            mCurrentEffect.FogColor = new Vector3(
                red,
                green,
                blue);
        }
#endif

        internal static void ForceSetColorOperation(ColorOperation value)
        {

            mLastColorOperationSet = value;



#if WINDOWS_PHONE || MONOGAME
            switch (value)
            {
                case FlatRedBall.Graphics.ColorOperation.Texture:
                case FlatRedBall.Graphics.ColorOperation.None:
                    mCurrentEffect.TextureEnabled = true;
                    mCurrentEffect.FogEnabled = false;



                    mCurrentEffect.VertexColorEnabled = true;
                    

                    break;
                case FlatRedBall.Graphics.ColorOperation.Add:
                    mCurrentEffect.TextureEnabled = true;
                    mCurrentEffect.VertexColorEnabled = false;
                    // This is handled in the emissive for the shader - or will be at least
                    mCurrentEffect.FogEnabled = false;

                    break;

                case FlatRedBall.Graphics.ColorOperation.Color:
                    mCurrentEffect.TextureEnabled = false;
                    mCurrentEffect.VertexColorEnabled = true;
                    mCurrentEffect.FogEnabled = false;
                    Renderer.Texture = null;

                    break;

                case FlatRedBall.Graphics.ColorOperation.ColorTextureAlpha:
                    mCurrentEffect.TextureEnabled = true;
                    mCurrentEffect.VertexColorEnabled = true;
#if MONOGAME
                    mCurrentEffect.FogEnabled = true;
                    mCurrentEffect.FogStart = 0;
                    mCurrentEffect.FogEnd = 1;
#endif
                    break;

                case FlatRedBall.Graphics.ColorOperation.Modulate:

                    mCurrentEffect.VertexColorEnabled = true;

                    mCurrentEffect.FogEnabled = false;
                    mCurrentEffect.TextureEnabled = true;

                    break;
                default:
                    throw new InvalidOperationException("The color operation " + value + " is not supported");

                    //break;
            }

#else
            string valueAsString;

            switch (value)
            {
                case ColorOperation.Add: valueAsString = "Add"; break;
                case ColorOperation.Color: valueAsString = "Color"; break;
                case ColorOperation.ColorTextureAlpha: valueAsString = "ColorTextureAlpha"; break;
                case ColorOperation.InverseTexture: valueAsString = "InverseTexture"; break;
                case ColorOperation.Modulate: valueAsString = "Modulate"; break;
                case ColorOperation.None: valueAsString = "Texture"; break; // None should just use Color
                case ColorOperation.Subtract: valueAsString = "Subtract"; break;
                case ColorOperation.Texture: valueAsString = "Texture"; break;
                case ColorOperation.Modulate2X: valueAsString = "Modulate2X"; break;
                case ColorOperation.Modulate4X: valueAsString = "Modulate4X"; break;
                case ColorOperation.InterpolateColor: valueAsString = "InterpolateColor"; break;
                default: valueAsString = StringEnum.GetStringValue(value); break;
            }

            EffectTechnique technique =
                mCurrentEffect.Techniques[valueAsString];

            if (technique == null)
            {
                string errorString =
                    "Could not find a technique named " + valueAsString +
                    " in the current shader.  If using a custom shader verify that " +
                    "this pixel shader technique exists.  Otherwise use a standard " +
                    "ColorOperation.";
            }
            else
            {
                mCurrentEffect.CurrentTechnique = technique;
                mColorOperation = valueAsString;
            }
#endif
        }


#if XNA4 || MONOGAME

        public static BlendState AddBlendState = new BlendState()
        {

#if WINDOWS_PHONE
            ColorSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.Max,
#endif

            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.One,
            AlphaBlendFunction = BlendFunction.Max,
        };

        public static BlendState RegularBlendState = new BlendState()
        {
#if WINDOWS_PHONE
            ColorSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            ColorBlendFunction = BlendFunction.ReverseSubtract,

#endif
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.InverseSourceAlpha,
            AlphaBlendFunction = BlendFunction.ReverseSubtract,

        };

#endif


        internal static void ForceSetBlendOperation()
        {
#if XNA4 || MONOGAME

            switch (mBlendOperation)
            {
                case FlatRedBall.Graphics.BlendOperation.Add:
#if MONOGAME
                    throw new NotImplementedException("Add color operation not supported in MonoGame");
#else
                    mGraphics.GraphicsDevice.BlendState = BlendState.Additive;
					break;
#endif
                case FlatRedBall.Graphics.BlendOperation.Regular:

//		#if ANDROID
//				mGraphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
//
//			#else
                    mGraphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
//		#endif
                    break;

                case FlatRedBall.Graphics.BlendOperation.NonPremultipliedAlpha:
                    mGraphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
                    break;
                case FlatRedBall.Graphics.BlendOperation.Modulate:
                    {
                        // Alpha doesn't draw correctly.
                        BlendState blendState = new BlendState();
                        blendState.AlphaSourceBlend = Blend.DestinationColor;
                        // Reach profile requires ColorSourceBlend to be the same as AlphaSourceBlend
                        blendState.ColorSourceBlend = Blend.DestinationColor;

                        blendState.AlphaDestinationBlend = Blend.Zero;
                        mGraphics.GraphicsDevice.BlendState = blendState;
                    }
                    break;
                case FlatRedBall.Graphics.BlendOperation.Modulate2X:
                    {
                        BlendState blendState = new BlendState();
                        blendState.AlphaSourceBlend = Blend.DestinationColor;
                        blendState.ColorSourceBlend = Blend.DestinationColor;

                        blendState.AlphaDestinationBlend = Blend.SourceColor;
                        blendState.ColorDestinationBlend = Blend.SourceColor;
                        mGraphics.GraphicsDevice.BlendState = blendState;
                    }
                    break;

                default:
                    throw new NotImplementedException("Blend operation not implemented: " + mBlendOperation);
                    //break;
            }
#else
            switch (mBlendOperation)
            {
                case BlendOperation.Add:
                    mGraphics.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
                    mGraphics.GraphicsDevice.RenderState.DestinationBlend = Blend.One;

                    break;
                case BlendOperation.Modulate:
                    mGraphics.GraphicsDevice.RenderState.SourceBlend = Blend.DestinationColor;
                    mGraphics.GraphicsDevice.RenderState.DestinationBlend = Blend.Zero;

                    break;
                case BlendOperation.Modulate2X:
                    mGraphics.GraphicsDevice.RenderState.SourceBlend = Blend.DestinationColor;
                    mGraphics.GraphicsDevice.RenderState.DestinationBlend = Blend.SourceColor;
                    break;

                case BlendOperation.Regular:
                    mGraphics.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
                    mGraphics.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
                    break;
            }

#endif
        }


        #endregion

        #region Private Methods

        #region Drawing Methods

        #region XML Docs
        /// <summary>
        /// Draws a quad
        /// The effect must already be started
        /// </summary>
        #endregion
        public static void DrawQuad(Vector3 bottomLeft, Vector3 topRight)
        {
            mQuadVertices = new VertexPositionTexture[] {
                new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 0)) };
            mQuadIndices = new short[] { 0, 1, 2, 2, 3, 0 };

#if XNA4 || MONOGAME
            mQuadVertexDeclaration = VertexPositionTexture.VertexDeclaration;
#else
            mQuadVertexDeclaration = new VertexDeclaration(FlatRedBallServices.GraphicsDevice, VertexPositionTexture.VertexElements);
#endif
            mQuadVertices[0].Position = new Vector3(topRight.X, bottomLeft.Y, 1);
            mQuadVertices[1].Position = new Vector3(bottomLeft.X, bottomLeft.Y, 1);
            mQuadVertices[2].Position = new Vector3(bottomLeft.X, topRight.Y, 1);
            mQuadVertices[3].Position = new Vector3(topRight.X, topRight.Y, 1);

#if XNA4 || MONOGAME
            throw new NotImplementedException();
#else
            FlatRedBallServices.GraphicsDevice.VertexDeclaration = mQuadVertexDeclaration;

            FlatRedBallServices.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(
                PrimitiveType.TriangleList, mQuadVertices,
                0, 4, mQuadIndices, 0, 2);

#endif

        }

        #region XML Docs
        /// <summary>
        /// Draws a full-screen quad
        /// The effect must already be started
        /// </summary>
        #endregion
        public static void DrawFullScreenQuad()
        {
            DrawQuad(Vector3.One * -1f, Vector3.One);
        }

        internal static void DrawZBufferedSprites(Camera camera, SpriteList listToRender)
        {
            // A note about 
            // how ZBuffered
            // rendering works
            // with alpha.  On the
            // PC, the FRB shaders use
            // a clip() function, which
            // prevents a pixel from being
            // processed.  In this case the 
            // FRB shader clips based off of
            // the Alpha in the Sprite.  If the
            // alpha is essentially 0, then the pixel
            // is not rendered and it does not modify the
            // depth buffer.  However, on other platforms where
            // we don't have shader access, the clip function isn't
            // available.
            // On Windows Phone 7 we use the AlphaTestEffect.  
            
            
            if (SpriteManager.ZBufferedSortType == SortType.Texture)
            {
                listToRender.SortTextureInsertion();
            }

            // Set device settings for drawing zbuffered sprites
            mVisibleSprites.Clear();
#if XNA4 || MONOGAME
            // vertex decleration not needed

            Renderer.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			if (camera.ClearsDepthBuffer)
			{
				Renderer.GraphicsDevice.DepthStencilState = DepthStencilState.Default;



			}
#else

			mGraphics.GraphicsDevice.VertexDeclaration = mPositionColorTexture;
            mGraphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            mGraphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            mGraphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

            // Pixels that have an alpha value less than this won't be drawn, therefore they won't impact
            // the depth buffer.  This is set to 1 so that fully-transparent pixels won't cover up pixels behind them
            // when using depth-buffered Sprites.
            mGraphics.GraphicsDevice.RenderState.ReferenceAlpha = 1;
#endif

            // Currently ZBuffered Sprites are all drawn - performance improvement
            // possible here by culling.


            lock (listToRender)
            {
                for (int i = 0; i < listToRender.Count; i++)
                {
                    Sprite s = listToRender[i];

                    if (s.AbsoluteVisible && s.Alpha > .0001f)
                    {
                        mVisibleSprites.Add(s);
                    }
                }
            }

            // Draw
            PrepareSprites(
                mZBufferedSpriteVertices, mZBufferedSpriteRenderBreaks,
                mVisibleSprites, 0, mVisibleSprites.Count);

            DrawSprites(
                mZBufferedSpriteVertices, mZBufferedSpriteRenderBreaks,
                mVisibleSprites, 0,
                mVisibleSprites.Count, camera);
        }

        private static void ClearBackgroundForLayer(Camera camera)
        {
#if XNA4
            Color clearColor = Color.Transparent;
#else
                Color clearColor = Color.TransparentBlack;
#endif
            if (camera.ClearsDepthBuffer)
            {
                mGraphics.GraphicsDevice.Clear(ClearOptions.DepthBuffer,
                        clearColor,
                        1, 0);
            }
        }

        static List<int> vertsPerVertexBuffer = new List<int>(4);

        private static void DrawShapes(Camera camera,
            PositionedObjectList<Sphere> spheres,
            PositionedObjectList<AxisAlignedCube> cubes,
            PositionedObjectList<AxisAlignedRectangle> rectangles,
            PositionedObjectList<Circle> circles,
            PositionedObjectList<Polygon> polygons,
            PositionedObjectList<Line> lines,
            PositionedObjectList<Capsule2D> capsule2Ds,
            Layer layer)
        {
            vertsPerVertexBuffer.Clear();
            TextureFilter oldFilter = FlatRedBallServices.GraphicsOptions.TextureFilter;

            FlatRedBallServices.GraphicsOptions.TextureFilter = TextureFilter.Linear;

            if (layer == null)
            {
                // reset the camera as it may have been set differently by layers
#if WINDOWS_PHONE || MONOGAME
                camera.SetDeviceViewAndProjection( mEffect, false);
                camera.SetDeviceViewAndProjection( mGenericEffect, false );
#else

                camera.SetDeviceViewAndProjection(mBasicEffect, false);
                camera.SetDeviceViewAndProjection(mEffect, false);
#endif
            }
            else
            {

#if WINDOWS_PHONE || MONOGAME
                camera.SetDeviceViewAndProjection( mEffect, layer.RelativeToCamera);
                camera.SetDeviceViewAndProjection( mGenericEffect, layer.RelativeToCamera );
#else
                camera.SetDeviceViewAndProjection(mBasicEffect, layer.RelativeToCamera);
                camera.SetDeviceViewAndProjection( mEffect, layer.RelativeToCamera);

#endif
            }
            #region 3D Shapes - these are different because we just render using FlatRedBall's built-in models in wireframe

            // Set up wireframe drawing
#if XNA4
            //throw new NotImplementedException();
            // TODO:  Set up the fill mode here
#else
            //GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
#endif
            //camera.SetDeviceViewAndProjection(mWireframeEffect, false);

            // Draw spheres
            // This is all done below now
            //for (int i = 0; i < spheres.Count; i++)
            //{
            //    mWireframeEffect.World =
            //        Matrix.CreateScale(spheres[i].Radius / 2f) *
            //        spheres[i].TransformationMatrix;

            //    mWireframeEffect.DiffuseColor =
            //        spheres[i].Color.ToVector3();

            //    foreach (ModelMesh mesh in mSphereShape.Meshes)
            //    {
            //        mesh.Draw();
            //    }
            //}

            // Draw cubes
            // This is all done below now
            //for (int i = 0; i < cubes.Count; i++)
            //{
            //    mWireframeEffect.World =
            //        Matrix.CreateScale(
            //            cubes[i].ScaleX, 
            //            cubes[i].ScaleY, 
            //            cubes[i].ScaleZ) *
            //        cubes[i].TransformationMatrix;

            //    mWireframeEffect.DiffuseColor =
            //        cubes[i].Color.ToVector3();

            //    foreach (ModelMesh mesh in mCubeShape.Meshes)
            //    {
            //        mesh.Draw();
            //    }
            //}

#if XNA4
            //throw new NotImplementedException();
            // TODO:  Turn off wireframe here
#else
            // End wireframe drawing
            //GraphicsDevice.RenderState.FillMode = FillMode.Solid;
#endif
            #endregion

#if XNA4 || MONOGAME
            //throw new NotImplementedException();
            // TODO:  Do the shape manager stuff here
            ColorOperation = FlatRedBall.Graphics.ColorOperation.Color;
            ForceSetColorOperation(FlatRedBall.Graphics.ColorOperation.Color);


#else
            mGraphics.GraphicsDevice.VertexDeclaration = mPositionColor;

            ShapeManager.FillPolygonVertexArrays(polygons);
            mGraphics.GraphicsDevice.RenderState.DepthBufferEnable = ShapeManager.UseZTestingWhenDrawing;
#endif
            bool hardwareInstanceCircles = false;

            if (hardwareInstanceCircles)
            {
                // TODO:  Maybe we do this someday?
            }
            else
            {
                #region Count the number of Vertices needed to draw the various shapes

                const int numberOfSphereSlices = 4;
                const int numberOfSphereVertsPerSlice = 17;
                int verticesToDraw = 0;

                // Rectangles require 5 points since the last is repeated
                for (int i = 0; i < rectangles.Count; i++)
                {
                    if (rectangles[i].AbsoluteVisible)
                    {
                        verticesToDraw += 5;
                    }
                }

                // add all the vertices for circles
                for (int i = 0; i < circles.Count; i++)
                {
                    if (circles[i].AbsoluteVisible)
                    {
                        verticesToDraw += ShapeManager.NumberOfVerticesForCircles;
                    }
                }

                // add all the vertices for the polygons
                verticesToDraw += ShapeManager.GetTotalPolygonVertexCount(polygons);

                // Add all the vertices for the lines
                verticesToDraw += lines.Count * 2;

                verticesToDraw += capsule2Ds.Count * ShapeManager.NumberOfVerticesForCapsule2Ds;

                // Add the vertices for AxisAlignedCubes
                verticesToDraw += cubes.Count * 16; // 16 points needed to draw a cube

                verticesToDraw += spheres.Count * numberOfSphereSlices * numberOfSphereVertsPerSlice;

                #endregion

                #region If there are no vertices to draw, then just return from the method.

                // if nothing is being drawn, exit the function now
                if (verticesToDraw != 0)
                {

                #endregion

                    #region Make sure that there are enough VertexBuffers created to hold how many vertices are needed.
                    // each vertex buffer holds 6000 vertices - this is common
                    // throughout FRB rendering
                    int numberOfVertexBuffers = 1 + (verticesToDraw / 6000);

                    while (mShapeVertices.Count < numberOfVertexBuffers)
                    {
                        mShapeVertices.Add(new VertexPositionColor[6000]);
                    }

                    #endregion

                    int vertexBufferNum = 0;
                    int vertNum = 0;

                    mRenderBreaks.Clear();

                    RenderBreak renderBreak = new RenderBreak(
                        0, null, ColorOperation.Color, BlendOperation.Regular, TextureAddressMode.Clamp);

#if DEBUG
                    renderBreak.ObjectCausingBreak = "ShapeManager";
#endif

                    mRenderBreaks.Add(renderBreak);

                    int renderBreakNumber = 0;

                    int verticesLeftToDraw = verticesToDraw;

                    #region Fill the vertArray with the Rectangle vertices
                    for (int i = 0; i < rectangles.Count; i++)
                    {
                        AxisAlignedRectangle rectangle = rectangles[i];

                        if (rectangle.AbsoluteVisible)
                        {
                            // since there are many kinds of shapes, check the vert num before each shape
                            if (vertNum + 5 > 6000)
                            {
                                //mShapesVertexBufferList[vertexBufferNum].SetData<VertexPositionColor>(mShapeVertices[vertexBufferNum], 0, vertNum, SetDataOptions.Discard);
                                vertexBufferNum++;

                                verticesLeftToDraw -= (vertNum);
                                vertsPerVertexBuffer.Add(vertNum);
                                vertNum = 0;
                            }

                            var buffer = mShapeVertices[vertexBufferNum];

                            buffer[vertNum + 0].Position =
                                new Vector3(
                                    rectangle.Left,
                                    rectangle.Top,
                                    rectangle.Z);
                            buffer[vertNum + 0].Color.PackedValue = rectangle.Color.PackedValue;

                            buffer[vertNum + 1].Position =
                                new Vector3(
                                    rectangle.Right,
                                    rectangle.Top,
                                    rectangle.Z);
                            buffer[vertNum + 1].Color.PackedValue = rectangle.Color.PackedValue;

                            buffer[vertNum + 2].Position =
                            new Vector3(
                                rectangle.Right,
                                rectangle.Bottom,
                                rectangle.Z);
                            buffer[vertNum + 2].Color.PackedValue = rectangle.Color.PackedValue;

                            buffer[vertNum + 3].Position =
                            new Vector3(
                                rectangle.Left,
                                rectangle.Bottom,
                                rectangle.Z);
                            buffer[vertNum + 3].Color.PackedValue = rectangle.Color.PackedValue;

                            buffer[vertNum + 4] = buffer[vertNum + 0];

                            vertNum += 5;
                            mRenderBreaks.Add(
                                    new RenderBreak((6000) * vertexBufferNum + vertNum,
                                    null, ColorOperation.Color, BlendOperation.Regular, TextureAddressMode.Clamp));
                            renderBreakNumber++;
                        }
                    }
                    #endregion

                    #region Fill the vert array with the Circle vertices

                    FlatRedBall.Math.Geometry.Circle circle;

                    // loop through all of the circles
                    for (int i = 0; i < circles.Count; i++)
                    {
                        circle = circles[i];

                        if (circle.AbsoluteVisible)
                        {
                            if (vertNum + ShapeManager.NumberOfVerticesForCircles > 6000)
                            {
                                //mShapesVertexBufferList[vertexBufferNum].SetData<VertexPositionColor>(mShapeVertices[vertexBufferNum], 0, vertNum, SetDataOptions.Discard);
                                vertexBufferNum++;

                                verticesLeftToDraw -= (vertNum);
                                vertsPerVertexBuffer.Add(vertNum);

                                vertNum = 0;
                            }

                            for (int pointNumber = 0; pointNumber < ShapeManager.NumberOfVerticesForCircles; pointNumber++)
                            {
                                float angle = pointNumber * 2 * 3.1415928f / (ShapeManager.NumberOfVerticesForCircles - 1);
                                mShapeVertices[vertexBufferNum][vertNum + pointNumber].Position =
                                    new Vector3(
                                        circle.Radius * (float)System.Math.Cos(angle) + circle.X,
                                        circle.Radius * (float)System.Math.Sin(angle) + circle.Y,
                                        circle.Z);
                                mShapeVertices[vertexBufferNum][vertNum + pointNumber].Color.PackedValue = circle.Color.PackedValue;
                            }
                            vertNum += ShapeManager.NumberOfVerticesForCircles;

                            renderBreak =
                                new RenderBreak((6000) * vertexBufferNum + vertNum,
                                null, ColorOperation.Color, BlendOperation.Regular, TextureAddressMode.Clamp);

#if DEBUG
                            renderBreak.ObjectCausingBreak = "Circle";
#endif

                            mRenderBreaks.Add(renderBreak);
                            renderBreakNumber++;
                        }
                    }
                    #endregion

                    #region Fill the vert array with the Capsule2D vertices

                    FlatRedBall.Math.Geometry.Capsule2D capsule2D;
                    int numberOfVerticesPerHalf = ShapeManager.NumberOfVerticesForCapsule2Ds / 2;
                    // This is the distance from the center of the Capsule to the center of the endpoint.
                    float endPointCenterDistanceX = 0;
                    float endPointCenterDistanceY = 0;
                    // Loop through all of the Capsule2Ds
                    for (int i = 0; i < capsule2Ds.Count; i++)
                    {
                        if (vertNum + ShapeManager.NumberOfVerticesForCapsule2Ds > 6000)
                        {
                            vertexBufferNum++;

                            verticesLeftToDraw -= (vertNum);
                            vertsPerVertexBuffer.Add(vertNum);

                            vertNum = 0;
                        }

                        capsule2D = capsule2Ds[i];

                        endPointCenterDistanceX = (float)(System.Math.Cos(capsule2D.RotationZ) * (capsule2D.mScale - capsule2D.mEndpointRadius));
                        endPointCenterDistanceY = (float)(System.Math.Sin(capsule2D.RotationZ) * (capsule2D.mScale - capsule2D.mEndpointRadius));

                        // First draw one half, then the draw the other half
                        for (int pointNumber = 0; pointNumber < numberOfVerticesPerHalf; pointNumber++)
                        {
                            float angle = capsule2D.RotationZ + -1.5707963f + pointNumber * 3.1415928f / (numberOfVerticesPerHalf - 1);


                            mShapeVertices[vertexBufferNum][vertNum + pointNumber].Position =
                                new Vector3(
                                    endPointCenterDistanceX + capsule2D.mEndpointRadius * (float)System.Math.Cos(angle) + capsule2D.X,
                                    endPointCenterDistanceY + capsule2D.mEndpointRadius * (float)System.Math.Sin(angle) + capsule2D.Y,
                                    capsule2D.Z);
                            mShapeVertices[vertexBufferNum][vertNum + pointNumber].Color.PackedValue = capsule2D.Color.PackedValue;
                        }

                        vertNum += numberOfVerticesPerHalf;

                        for (int pointNumber = 0; pointNumber < numberOfVerticesPerHalf; pointNumber++)
                        {
                            float angle = capsule2D.RotationZ + 1.5707963f + pointNumber * 3.1415928f / (numberOfVerticesPerHalf - 1);


                            mShapeVertices[vertexBufferNum][vertNum + pointNumber].Position =
                                new Vector3(
                                    -endPointCenterDistanceX + capsule2D.mEndpointRadius * (float)System.Math.Cos(angle) + capsule2D.X,
                                    -endPointCenterDistanceY + capsule2D.mEndpointRadius * (float)System.Math.Sin(angle) + capsule2D.Y,
                                    capsule2D.Z);
                            mShapeVertices[vertexBufferNum][vertNum + pointNumber].Color.PackedValue = capsule2D.Color.PackedValue;
                        }
                        vertNum += numberOfVerticesPerHalf;

                        mShapeVertices[vertexBufferNum][vertNum] =
                            mShapeVertices[vertexBufferNum][vertNum - 2 * numberOfVerticesPerHalf];

                        vertNum++;

                        renderBreak =
                            new RenderBreak((6000) * vertexBufferNum + vertNum,
                            null, ColorOperation.Color, BlendOperation.Regular, TextureAddressMode.Clamp);

#if DEBUG
                        renderBreak.ObjectCausingBreak = "Capsule";
#endif

                        mRenderBreaks.Add(renderBreak);
                        renderBreakNumber++;
                    }

                    #endregion

                    #region Fill the vertArray with the polygon vertices

                    for (int i = 0; i < polygons.Count; i++)
                    {
                        if (polygons[i].Points != null && polygons[i].Points.Count > 1)
                        {
                            // if this polygon knocks us into the next vertex buffer, then set the data for this one, then move on
                            if (vertNum + polygons[i].Points.Count > 6000)
                            {
                                //mShapesVertexBufferList[vertexBufferNum].SetData<VertexPositionColor>(mShapeVertices[vertexBufferNum], 0, vertNum, SetDataOptions.Discard);
                                vertexBufferNum++;

                                verticesLeftToDraw -= (vertNum);
                                vertsPerVertexBuffer.Add(vertNum);

                                vertNum = 0;
                            }

                            polygons[i].Vertices.CopyTo(mShapeVertices[vertexBufferNum], vertNum);
                            vertNum += polygons[i].Vertices.Length;

                            renderBreak =
                                new RenderBreak((6000) * vertexBufferNum + vertNum,
                                null, ColorOperation.Color, BlendOperation.Regular, TextureAddressMode.Clamp);

#if DEBUG
                            renderBreak.ObjectCausingBreak = "Polygon";
#endif


                            mRenderBreaks.Add(renderBreak);
                            renderBreakNumber++;
                        }
                    }


                    #endregion

                    #region Fill the vertArray with the line vertices

                    for (int i = 0; i < lines.Count; i++)
                    {
                        // if this line knocks us into the next vertex buffer, then set the data for this one, then move on
                        if (vertNum + 2 > 6000)
                        {
                            //mShapesVertexBufferList[vertexBufferNum].SetData<VertexPositionColor>(mShapeVertices[vertexBufferNum], 0, vertNum, SetDataOptions.Discard);
                            vertexBufferNum++;
                            verticesLeftToDraw -= vertNum;
                            vertsPerVertexBuffer.Add(vertNum);

                            vertNum = 0;
                        }

                        // Add the line points
                        mShapeVertices[vertexBufferNum][vertNum + 0].Position =
                            lines[i].Position +
                            Vector3.Transform(new Vector3(
                                (float)lines[i].RelativePoint1.X,
                                (float)lines[i].RelativePoint1.Y,
                                (float)lines[i].RelativePoint1.Z),
                                lines[i].RotationMatrix);
                        mShapeVertices[vertexBufferNum][vertNum + 0].Color.PackedValue = lines[i].Color.PackedValue;

                        mShapeVertices[vertexBufferNum][vertNum + 1].Position =
                            lines[i].Position +
                            Vector3.Transform(new Vector3(
                                (float)lines[i].RelativePoint2.X,
                                (float)lines[i].RelativePoint2.Y,
                                (float)lines[i].RelativePoint2.Z),
                                lines[i].RotationMatrix);
                        mShapeVertices[vertexBufferNum][vertNum + 1].Color.PackedValue = lines[i].Color.PackedValue;

                        // Increment the vertex number past this line
                        vertNum += 2;

                        // Add a render break

                        renderBreak =
                            new RenderBreak((6000) * vertexBufferNum + vertNum,
                            null, ColorOperation.Color, BlendOperation.Regular, TextureAddressMode.Clamp);

#if DEBUG
                        renderBreak.ObjectCausingBreak = "Line";
#endif

                        mRenderBreaks.Add(renderBreak);
                        renderBreakNumber++;

                    }

                    #endregion

                    #region Fill the vertArray with the AxisAlignedCube pieces

                    for (int i = 0; i < cubes.Count; i++)
                    {
                        AxisAlignedCube cube = cubes[i];

                        const int numberOfPoints = 16;

                        if (vertNum + numberOfPoints > 6000)
                        {
                            vertexBufferNum++;

                            verticesLeftToDraw -= (vertNum);
                            vertsPerVertexBuffer.Add(vertNum);

                            vertNum = 0;
                        }

                        // We can do the top/bottom all in one pass
                        for (int cubeVertIndex = 0; cubeVertIndex < 16; cubeVertIndex++)
                        {
                            mShapeVertices[vertexBufferNum][vertNum + cubeVertIndex].Position = new Vector3(
                                ShapeManager.UnscaledCubePoints[cubeVertIndex].X * cube.mScaleX + cube.Position.X,
                                ShapeManager.UnscaledCubePoints[cubeVertIndex].Y * cube.mScaleY + cube.Position.Y,
                                ShapeManager.UnscaledCubePoints[cubeVertIndex].Z * cube.mScaleZ + cube.Position.Z);

                            mShapeVertices[vertexBufferNum][vertNum + cubeVertIndex].Color = cube.mColor;

                            if (cubeVertIndex == 9 || cubeVertIndex == 11 || cubeVertIndex == 13 || cubeVertIndex == 15)
                            {
                                renderBreak =
                                        new RenderBreak((6000) * vertexBufferNum + vertNum + cubeVertIndex + 1,
                                        null, ColorOperation.Color, BlendOperation.Regular, TextureAddressMode.Clamp);

#if DEBUG
                                renderBreak.ObjectCausingBreak = "Cube";
#endif

                                mRenderBreaks.Add(renderBreak);

                                renderBreakNumber++;
                            }
                        }

                        vertNum += numberOfPoints;


                    }


                    #endregion



                    #region Fill the vertArray with the Sphere pieces

                    for (int sphereIndex = 0; sphereIndex < spheres.Count; sphereIndex++)
                    {

                        if (vertNum + numberOfSphereSlices * numberOfSphereVertsPerSlice > 6000)
                        {
                            //mShapesVertexBufferList[vertexBufferNum].SetData<VertexPositionColor>(mShapeVertices[vertexBufferNum], 0, vertNum, SetDataOptions.Discard);
                            vertexBufferNum++;

                            verticesLeftToDraw -= (vertNum);
                            vertsPerVertexBuffer.Add(vertNum);

                            vertNum = 0;
                        }

                        Sphere sphere = spheres[sphereIndex];


                        for (int sliceNumber = 0; sliceNumber < numberOfSphereSlices; sliceNumber++)
                        {
                            float sliceAngle = sliceNumber * 3.1415928f / ((float)numberOfSphereSlices);
                            Matrix rotationMatrix = Matrix.CreateRotationY(sliceAngle);

                            for (int pointNumber = 0; pointNumber < numberOfSphereVertsPerSlice; pointNumber++)
                            {
                                float angle = pointNumber * 2 * 3.1415928f / (numberOfSphereVertsPerSlice - 1);

                                Vector3 newPosition = new Vector3(
                                        sphere.Radius * (float)System.Math.Cos(angle),
                                        sphere.Radius * (float)System.Math.Sin(angle), 0);

                                MathFunctions.TransformVector(ref newPosition, ref rotationMatrix);

                                newPosition += sphere.Position;

                                mShapeVertices[vertexBufferNum][vertNum + pointNumber].Position = newPosition;

                                mShapeVertices[vertexBufferNum][vertNum + pointNumber].Color.PackedValue = sphere.Color.PackedValue;
                            }

                            vertNum += numberOfSphereVertsPerSlice;

                            renderBreak = new RenderBreak((6000) * vertexBufferNum + vertNum,
                                null, ColorOperation.Color, BlendOperation.Regular, TextureAddressMode.Clamp);

#if DEBUG
                            renderBreak.ObjectCausingBreak = "Sphere";
#endif

                            mRenderBreaks.Add(renderBreak);
                        }

                    }

                    #endregion


                    //            vertexBufferList[vertexBufferNum].SetData<VertexPositionColor>(
                    //              mShapeDrawingVertexArray, 0, verticesToDraw, SetDataOptions.None);

                    //                if (verticesLeftToDraw != 0)
                    //                {
                    //#if XBOX360
                    //                    mShapesVertexBufferList[vertexBufferNum].SetData<VertexPositionColor>(mShapeDrawingVertexArray);
                    //#else
                    //                    mShapesVertexBufferList[vertexBufferNum].SetData<VertexPositionColor>(mShapeDrawingVertexArray, 0, verticesLeftToDraw, SetDataOptions.Discard);
                    //#endif
                    //                }

                    int vertexBufferIndex = 0;
                    int renderBreakIndexForRendering = 0;
                    for (; vertexBufferIndex < vertsPerVertexBuffer.Count; vertexBufferIndex++)
                    {
                        DrawVertexList<VertexPositionColor>(camera, mShapeVertices, mRenderBreaks,
                            vertsPerVertexBuffer[vertexBufferIndex], PrimitiveType.LineStrip,
                            6000, vertexBufferIndex, ref renderBreakIndexForRendering);
                    }
                    DrawVertexList<VertexPositionColor>(camera, mShapeVertices, mRenderBreaks, 
                        verticesLeftToDraw, PrimitiveType.LineStrip,
                        6000, vertexBufferIndex, ref renderBreakIndexForRendering);

                }
                //DrawVBList(camera, mShapesVertexBufferList, mRenderBreaks, verticesToDraw, PrimitiveType.LineStrip, VertexPositionColor.SizeInBytes);
            }

            FlatRedBallServices.GraphicsOptions.TextureFilter = oldFilter;
        }

        #endregion

        #region Helper Drawing Methods

        private static void DrawMixedStart(Camera camera)
        {
            Renderer.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            if (camera.ClearsDepthBuffer)
            {
                Renderer.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            }
#if MONOGAME
            mCurrentEffect.LightingEnabled = false;
#endif


       }

        private static void PrepareSprites(
            List<VertexPositionColorTexture[]> spriteVertices,
            List<RenderBreak> renderBreaks,
            IList<Sprite> spritesToDraw, int startIndex, int numberOfVisible)
        {
            // Make sure there are enough vertex arrays
            int numberOfVertexArrays = 1 + (numberOfVisible / 1000);
            while (spriteVertices.Count < numberOfVertexArrays)
            {
                spriteVertices.Add(new VertexPositionColorTexture[6000]);
            }

            // Clear the render breaks
            renderBreaks.Clear();

            FillVertexList(spritesToDraw, spriteVertices,
                renderBreaks, startIndex, numberOfVisible);

            mRenderBreaksAllocatedThisFrame += renderBreaks.Count;

            if (RecordRenderBreaks)
            {
                LastFrameRenderBreakList.AddRange(renderBreaks);
            }
               

        }

        private static int DrawSprites(
            List<VertexPositionColorTexture[]> spriteVertices,
            List<RenderBreak> renderBreaks,
            IList<Sprite> spritesToDraw, int startIndex, int numberOfVisibleSprites, Camera camera)
        {
            // Prepare device settings

#if XNA4 || MONOGAME
            // TODO:  Turn off cull mode:
            TextureAddressMode mode = TextureAddressMode;
            if( spritesToDraw.Count > 0 )
                TextureAddressMode = spritesToDraw[0].TextureAddressMode;
#else
            mGraphics.GraphicsDevice.RenderState.CullMode = CullMode.None;
            mGraphics.GraphicsDevice.VertexDeclaration = mPositionColorTexture;
#endif
            #region Old Code

            //int numberOfVertexBuffers = 1 + (numToDraw / 1000);

            //mSpriteRenderBreaks.Clear();

            //while (mSpriteVertices.Count < numberOfVertexBuffers)
            //{
            //    mSpriteVertices.Add(new VertexPositionColorTexture[6000]);
            //}

            ////while (mVertexBufferList.Count < numberOfVertexBuffers)
            ////{
            ////    mVertexBufferList.Add(new DynamicVertexBuffer(
            ////            Graphics.GraphicsDevice, 
            ////            6000 * VertexPositionColorTexture.SizeInBytes, 
            ////            BufferUsage.None));
            ////}

            //// numToDraw is the number of visible Sprites, not the range
            //int numberOfVisibleSprites = numToDraw;

            //// this function can change the numToDraw
            //FillVertexList(spritesToDraw, mSpriteVertices,
            //    camera, mSpriteRenderBreaks, startIndex, ref numToDraw);
            ////FillVBList(spritesToDraw, mVertexBufferList,
            ////    camera, mRenderBreaks, startIndex, ref numToDraw);
            //mRenderBreaksAllocatedThisFrame += mRenderBreaks.Count;

            //// now numToDraw is the range which is equal to or greater than the value before the call

            #endregion

            // numberToRender * 2 represents how many triangles.  Therefore we only want to use the number of visible Sprites
            DrawVertexList<VertexPositionColorTexture>(camera, spriteVertices, renderBreaks,
                numberOfVisibleSprites * 2, PrimitiveType.TriangleList, 6000);
            //DrawVBList(camera, mVertexBufferList, mRenderBreaks,
            //    numberOfVisibleSprites * 2, PrimitiveType.TriangleList, VertexPositionColorTexture.SizeInBytes);
#if XNA4 || MONOGAME
            TextureAddressMode = mode;
#endif
            return numberOfVisibleSprites;
        }

        private static void DrawTexts(List<Text> texts, int startIndex,
            int numToDraw, Camera camera, Section section)
        {


            if (TextManager.UseNativeTextRendering)
            {
#if XNA4 || SILVERLIGHT
                TextManager.DrawTexts(texts, startIndex, numToDraw, camera);
#endif
            }
            else
            {
                    #region Bitmap Font Rendering






#if XNA4 || MONOGAME
                    // no need to do vertex declerations in XNA 4
#else
            mGraphics.GraphicsDevice.VertexDeclaration = mPositionColorTexture;
#endif
                    int totalVertices = 0;

                    for (int i = startIndex; i < startIndex + numToDraw; i++)
                    {

                        if (!texts[i].RenderOnTexture)
                            totalVertices += texts[i].VertexCount;
                    }

                    if (totalVertices == 0)
                    {
                        return;

                    }
                    TextureFilter oldTextureFilter = FlatRedBallServices.GraphicsOptions.TextureFilter;

                    if (TextManager.FilterTexts == false)
                    {
                        FlatRedBallServices.GraphicsOptions.TextureFilter = TextureFilter.Point;
                    }
                    int numberOfVertexBuffers = 1 + (totalVertices / 6000);

                    mTextRenderBreaks.Clear();

                    //      DrawTextsVertexBufferList.Clear();

                    // If there are not enough Vertex Buffers to hold all of the vertices
                    // for drawing the texts, add more
                    while (mTextVertices.Count < numberOfVertexBuffers)
                    {
                        mTextVertices.Add(new VertexPositionColorTexture[6000]);
                    }
                    //for (int i = mVertexBufferList.Count; i < numberOfVertexBuffers; i++)
                    //{
                    //    mVertexBufferList.Add(
                    //        new DynamicVertexBuffer(
                    //            Graphics.GraphicsDevice, 6000 * VertexPositionColorTexture.SizeInBytes, BufferUsage.None));
                    //}

                    int numberToRender =
                        FillVertexList(texts, mTextVertices,
                        camera, mTextRenderBreaks, startIndex, numToDraw, totalVertices);
                    //FillVBList(texts, mVertexBufferList,
                    //camera, mRenderBreaks, startIndex, numToDraw, totalVertices);
                    mRenderBreaksAllocatedThisFrame += mTextRenderBreaks.Count;

                    if (RecordRenderBreaks)
                    {
                        LastFrameRenderBreakList.AddRange(mTextRenderBreaks);
                    }

                    DrawVertexList<VertexPositionColorTexture>(camera, mTextVertices, mTextRenderBreaks,
                        totalVertices / 3, PrimitiveType.TriangleList, 6000);

                    //DrawVBList(camera, mVertexBufferList, mRenderBreaks,
                    //    totalVertices/3, PrimitiveType.TriangleList, VertexPositionColorTexture.SizeInBytes);

                    //          for (int i = 0; i < DrawTextsVertexBufferList.Count; i++)
                    //            DrawTextsVertexBufferList[i].Dispose();
                FlatRedBallServices.GraphicsOptions.TextureFilter = oldTextureFilter;
                #endregion
            }
        }

        //public static void DrawVBList(Camera camera,
        //    List<DynamicVertexBuffer> vertexBufferList, List<RenderBreak> renderBreaks,
        //    int numberOfPrimitives, PrimitiveType primitiveType,
        //    int vertexSizeInBytes)
        //{
        //    DrawVBList(camera, vertexBufferList, renderBreaks, numberOfPrimitives, primitiveType,
        //        vertexSizeInBytes, 6000);
        //}

        public static void DrawVertexList<T>(Camera camera,
            List<T[]> vertexList, List<RenderBreak> renderBreaks,
            int numberOfPrimitives, PrimitiveType primitiveType,
            //int vertexSizeInBytes, 
            int verticesPerVertexBuffer) where T : struct
#if XNA4 || MONOGAME
            , IVertexType
#endif
        {
            int throwAway = 0;
            DrawVertexList(camera, vertexList, renderBreaks, numberOfPrimitives, primitiveType, verticesPerVertexBuffer, 0, ref throwAway);
        }



        public static void DrawVertexList<T>(Camera camera,
            List<T[]> vertexList, List<RenderBreak> renderBreaks,
            int numberOfPrimitives, PrimitiveType primitiveType,
            //int vertexSizeInBytes, 
            int verticesPerVertexBuffer, int vbIndex, ref int renderBreakIndex) where T : struct
#if XNA4 || MONOGAME
            , IVertexType
#endif
        {

            bool startedOnNonZeroVBIndex = vbIndex != 0;

            if (numberOfPrimitives == 0) return;

            TextureAddressMode mode = TextureAddressMode;

            #region Get the verticesPerPrimitive and extraVertices according to the PrimitiveType
            int verticesPerPrimitive = 1;

            // some primitive types, like LineStrip, require 1 extra vertex for the initial point.
            // that is, to draw 3 lines, 4 points are needed.  This variable is used for that
            int extraVertices = 0;

            switch (primitiveType)
            {

#if !XNA4 && !MONOGAME
                case PrimitiveType.PointList:
                    verticesPerPrimitive = 1;
                    break;
#endif
                case PrimitiveType.TriangleList:
                    verticesPerPrimitive = 3;
                    break;
                case PrimitiveType.LineStrip:
                    verticesPerPrimitive = 1;
                    extraVertices = 1;
                    break;
            }
            #endregion

            int drawToOnThisVB = 0;
            int drawnOnThisVB = 0;
            int totalDrawn = 0;

            int VBOn = vbIndex;

            //DynamicVertexBuffer tempVB = vertexBufferList[0];

            //mGraphics.GraphicsDevice.Vertices[0].SetSource(tempVB, 0, vertexSizeInBytes);


            int numPasses = 0;

            int numberOfPrimitivesPerVertexBuffer = verticesPerVertexBuffer / verticesPerPrimitive;
#if !WINDOWS_PHONE && !MONOGAME
            Microsoft.Xna.Framework.Graphics.Effect effectToUse = mCurrentEffect;
#else
            GenericEffect effectToUse = mCurrentEffect;
#endif

            if (primitiveType == PrimitiveType.LineStrip)
            {

#if !WINDOWS_PHONE && !MONOGAME
                mBasicEffect.LightingEnabled = false;
                mBasicEffect.VertexColorEnabled = true;
                effectToUse = mBasicEffect;
#else
                mGenericEffect.LightingEnabled = false;
                mGenericEffect.VertexColorEnabled = true;
                effectToUse = mGenericEffect;
#endif
            }

            if (renderBreaks.Count != 0)
            {
                renderBreaks[renderBreakIndex].SetStates();
                renderBreakIndex++;
            }
            #region move through all of the VBs and draw them

            TextureAddressMode modeToCheck = TextureAddressMode;
            while (totalDrawn < numberOfPrimitives)
            {
                numPasses++;
                //Changed because erraneous tiles were being drawn.  Likely due to uncleared data from VB.

                if (startedOnNonZeroVBIndex)
                {
                    drawToOnThisVB = System.Math.Min(numberOfPrimitivesPerVertexBuffer, (numberOfPrimitives));

                }
                else
                {
                    drawToOnThisVB = System.Math.Min(numberOfPrimitivesPerVertexBuffer, (numberOfPrimitives - (numberOfPrimitivesPerVertexBuffer * VBOn)));
                }

                if (drawToOnThisVB < 0)
                {
                    drawToOnThisVB = numberOfPrimitives;
                }

                if (renderBreakIndex < renderBreaks.Count && ((RenderBreak)renderBreaks[renderBreakIndex]).ItemNumber < numberOfPrimitivesPerVertexBuffer * VBOn + drawToOnThisVB)
                {
                    drawToOnThisVB = renderBreaks[renderBreakIndex].ItemNumber - (numberOfPrimitivesPerVertexBuffer * VBOn);
                    //drawToOnThisVB = renderBreaks[renderBreakIndex].ItemNumber;
#if WINDOWS_PHONE || MONOGAME

                    //effectToUse = new BasicEffect(GraphicsDevice);

                    //((BasicEffect)effectToUse).TextureEnabled = false;
                    //((BasicEffect)effectToUse).LightingEnabled = false;
                    //((BasicEffect)effectToUse).VertexColorEnabled = false;
                    //((BasicEffect)effectToUse).FogEnabled = false;

                    //VertexPositionColorTexture[] array = new VertexPositionColorTexture[6];

                    //foreach (EffectPass pass in effectToUse.CurrentTechnique.Passes)
                    //{
                    //    pass.Apply();

                    //    mGraphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(
                    //        PrimitiveType.TriangleList,
                    //        array,
                    //        0,
                    //        2);
                    //}

                    //effectToUse.Dispose();


                    EffectTechnique currentTechnique = effectToUse.GetCurrentTechnique(true);
                    foreach (EffectPass pass in currentTechnique.Passes)
                    {
                        pass.Apply();
                        if (drawToOnThisVB != drawnOnThisVB)
                        {
                            GraphicsDevice device = mGraphics.GraphicsDevice;
                            T[] verts = vertexList[VBOn];

                            int start = verticesPerPrimitive * drawnOnThisVB;
                            int numberToDraw = drawToOnThisVB - drawnOnThisVB - extraVertices;
                            device.DrawUserPrimitives<T>(
                                primitiveType,
                                verts,
                                start,
                                numberToDraw);


                        }
                    }
#elif XNA4 && !WINDOWS_PHONE && !MONODROID
                    EffectTechnique currentTechnique = effectToUse.CurrentTechnique;
                    foreach (EffectPass pass in currentTechnique.Passes)
                    {
                        pass.Apply();

                        if (drawToOnThisVB != drawnOnThisVB)
                        {

                            mGraphics.GraphicsDevice.DrawUserPrimitives<T>(
                                primitiveType,
                                vertexList[VBOn],
                                verticesPerPrimitive * drawnOnThisVB,
                                drawToOnThisVB - drawnOnThisVB - extraVertices,
                                vertexList[VBOn][0].VertexDeclaration);
                        }
                    }


#else
                    effectToUse.Begin();

                    for (int i = 0; i < effectToUse.CurrentTechnique.Passes.Count; i++)
                    {
                        EffectPass pass = effectToUse.CurrentTechnique.Passes[i];
                        pass.Begin();

                        if (drawToOnThisVB != drawnOnThisVB)
                            mGraphics.GraphicsDevice.DrawUserPrimitives<T>(
                                primitiveType,
                                vertexList[VBOn],
                                verticesPerPrimitive * drawnOnThisVB,
                                drawToOnThisVB - drawnOnThisVB - extraVertices);
                        //mGraphics.GraphicsDevice.DrawPrimitives(
                        //    primitiveType,
                        //    verticesPerPrimitive * drawnOnThisVB,
                        //    drawToOnThisVB - drawnOnThisVB - extraVertices);

                        pass.End();
                    }

                    effectToUse.End();
#endif

                    renderBreaks[renderBreakIndex - 1].Cleanup();
                    renderBreaks[renderBreakIndex].SetStates();

                    renderBreakIndex++;
                }
                else
                {
#if WINDOWS_PHONE || MONOGAME
                    EffectTechnique currentTechnique = effectToUse.GetCurrentTechnique(true);
                    foreach (EffectPass pass in currentTechnique.Passes)
                    {
                        pass.Apply();

                        if (drawToOnThisVB - extraVertices != drawnOnThisVB)
                        {
                            mGraphics.GraphicsDevice.DrawUserPrimitives<T>(
                                primitiveType,
                                vertexList[VBOn],
                                verticesPerPrimitive * drawnOnThisVB,
                                drawToOnThisVB - drawnOnThisVB - extraVertices);
                        }
                    }
#elif XNA4 && !WINDOWS_PHONE && !MONODROID
                    EffectTechnique currentTechnique = effectToUse.CurrentTechnique;
                    foreach (EffectPass pass in currentTechnique.Passes)
                    {
                        pass.Apply();

                        if (drawToOnThisVB - extraVertices != drawnOnThisVB)
                        {
                            mGraphics.GraphicsDevice.DrawUserPrimitives<T>(
                                primitiveType,
                                vertexList[VBOn],
                                verticesPerPrimitive * drawnOnThisVB,
                                drawToOnThisVB - drawnOnThisVB - extraVertices);
                        }
                    }
#else
                    effectToUse.Begin();

                    for (int i = 0; i < effectToUse.CurrentTechnique.Passes.Count; i++)
                    {
                        EffectPass pass = effectToUse.CurrentTechnique.Passes[i];
                        pass.Begin();

                        // The 3rd argument (drawToOnThisVB - drawnOnThisVB) is the number of triangles, not vertices.
                        if (drawToOnThisVB - extraVertices != drawnOnThisVB)
                        {
                            mGraphics.GraphicsDevice.DrawUserPrimitives<T>(
                                primitiveType,
                                vertexList[VBOn],
                                verticesPerPrimitive * drawnOnThisVB,
                                drawToOnThisVB - drawnOnThisVB - extraVertices);
                        }
                        //mGraphics.GraphicsDevice.DrawPrimitives(
                        //    primitiveType,
                        //    verticesPerPrimitive * drawnOnThisVB,
                        //    drawToOnThisVB - drawnOnThisVB - extraVertices);

                        pass.End();
                    }

                    effectToUse.End();
#endif
                    renderBreaks[renderBreakIndex - 1].Cleanup();
                }

                totalDrawn += drawToOnThisVB - drawnOnThisVB;

                drawnOnThisVB = drawToOnThisVB;

                

                if (drawToOnThisVB == numberOfPrimitivesPerVertexBuffer && totalDrawn < numberOfPrimitives)
                {
                    VBOn++;
                    drawnOnThisVB = 0;
                    //tempVB = vertexBufferList[VBOn];
                    //mGraphics.GraphicsDevice.Vertices[0].SetSource(tempVB, 0, vertexSizeInBytes);
                }
            }

            if( TextureAddressMode != mode )
                TextureAddressMode = mode;
            #endregion
        }

        //public static void DrawVBList(Camera camera,
        //    List<DynamicVertexBuffer> vertexBufferList, List<RenderBreak> renderBreaks,
        //    int numberOfPrimitives, PrimitiveType primitiveType,
        //    int vertexSizeInBytes, int verticesPerVertexBuffer)            
        //{

        //    if (numberOfPrimitives == 0) return;

        //    #region Get the verticesPerPrimitive and extraVertices according to the PrimitiveType
        //    int verticesPerPrimitive = 1;

        //    // some primitive types, like LineStrip, require 1 extra vertex for the initial point.
        //    // that is, to draw 3 lines, 4 points are needed.  This variable is used for that
        //    int extraVertices = 0;

        //    switch (primitiveType)
        //    {
        //        case PrimitiveType.PointList:
        //            verticesPerPrimitive = 1;
        //            break;
        //        case PrimitiveType.TriangleList:
        //            verticesPerPrimitive = 3;
        //            break;
        //        case PrimitiveType.LineStrip:
        //            verticesPerPrimitive = 1;
        //            extraVertices = 1;
        //            break;
        //    }
        //    #endregion

        //    if (renderBreaks.Count != 0)
        //    {
        //        renderBreaks[0].SetStates();
        //    }

        //    int drawToOnThisVB = 0;
        //    int drawnOnThisVB = 0;
        //    int totalDrawn = 0;

        //    int batchBreakNumber = 1;

        //    int VBOn = 0;

        //    DynamicVertexBuffer tempVB = vertexBufferList[0];

        //    mGraphics.GraphicsDevice.Vertices[0].SetSource(tempVB, 0, vertexSizeInBytes);

        //    int numPasses = 0;

        //    int numberOfPrimitivesPerVertexBuffer = verticesPerVertexBuffer / verticesPerPrimitive;

        //    Microsoft.Xna.Framework.Graphics.Effect effectToUse = mCurrentEffect;

        //    if (primitiveType == PrimitiveType.LineStrip)
        //    {
        //        mBasicEffect.VertexColorEnabled = true;

        //        camera.SetDeviceViewAndProjection(mBasicEffect, false);
        //        effectToUse = mBasicEffect;
        //    }

        //    #region move through all of the VBs and draw them
        //    while (totalDrawn < numberOfPrimitives)
        //    {
        //        numPasses++;
        //        drawToOnThisVB = System.Math.Min(numberOfPrimitivesPerVertexBuffer, (numberOfPrimitives - (numberOfPrimitivesPerVertexBuffer * VBOn)));

        //        if (batchBreakNumber < renderBreaks.Count && ((RenderBreak)renderBreaks[batchBreakNumber]).ItemNumber < numberOfPrimitivesPerVertexBuffer * VBOn + drawToOnThisVB)
        //        {
        //            drawToOnThisVB = renderBreaks[batchBreakNumber].ItemNumber - (numberOfPrimitivesPerVertexBuffer * VBOn);

        //            effectToUse.Begin();

        //            for (int i = 0; i < effectToUse.CurrentTechnique.Passes.Count; i++)
        //            {
        //                EffectPass pass = effectToUse.CurrentTechnique.Passes[i];
        //                pass.Begin();
        //                if (drawToOnThisVB != drawnOnThisVB)
        //                    mGraphics.GraphicsDevice.DrawPrimitives(
        //                        primitiveType,
        //                        verticesPerPrimitive * drawnOnThisVB,
        //                        drawToOnThisVB - drawnOnThisVB - extraVertices);

        //                pass.End();
        //            }

        //            effectToUse.End();

        //            renderBreaks[batchBreakNumber].SetStates();

        //            batchBreakNumber++;
        //        }
        //        else
        //        {
        //            effectToUse.Begin();

        //            for (int i = 0; i < effectToUse.CurrentTechnique.Passes.Count; i++)
        //            {
        //                EffectPass pass = effectToUse.CurrentTechnique.Passes[i];
        //                pass.Begin();

        //                // The 3rd argument (drawToOnThisVB - drawnOnThisVB) is the number of triangles, not vertices.
        //                mGraphics.GraphicsDevice.DrawPrimitives(
        //                    primitiveType,
        //                    verticesPerPrimitive * drawnOnThisVB,
        //                    drawToOnThisVB - drawnOnThisVB - extraVertices);

        //                pass.End();
        //            }

        //            effectToUse.End();
        //        }

        //        totalDrawn += drawToOnThisVB - drawnOnThisVB;

        //        drawnOnThisVB = drawToOnThisVB;

        //        if (drawToOnThisVB == numberOfPrimitivesPerVertexBuffer && totalDrawn < numberOfPrimitives)
        //        {
        //            VBOn++;
        //            drawnOnThisVB = 0;
        //            tempVB = vertexBufferList[VBOn];
        //            mGraphics.GraphicsDevice.Vertices[0].SetSource(tempVB, 0, vertexSizeInBytes);
        //        }
        //    }

        //    #endregion
        //}

        #endregion

        #region Vertex Buffer Helpers

        internal static void SetNumberOfThreadsToUse(int numberOfUpdaters)
        {

            mFillVertexLogics.Clear();

            for (int i = 0; i < numberOfUpdaters; i++)
            {
                FillVertexLogic logic = new FillVertexLogic();
                mFillVertexLogics.Add(logic);
            }
        }


        private static void FillVertexList(IList<Sprite> sa,
            List<VertexPositionColorTexture[]> vertexLists,
            List<RenderBreak> renderBreaks, int firstSprite,
            int numberToDraw)
        {
            mFillVBListCallsThisFrame++;

            ////////////////////Early Out/////////////////////////////
            #region if the Array is empty, then we just exit.
            if (sa.Count == 0) return;
            #endregion
            ///////////////////End Early Out////////////////////////

            // clear the places where batching breaks occur.
            renderBreaks.Clear();

            //Visual Studio says we never use this so I'm taking it out:
            //int vertNum = 0;

            int vertexBufferNum = 0;

            int vertNumForRenderBreaks = 0;
            int vertexBufferNumForRenderBreaks = 0;

            VertexPositionColorTexture[] arrayAtIndex = vertexLists[vertexBufferNum];

            RenderBreak renderBreak = new RenderBreak(firstSprite, sa[firstSprite]);

            renderBreaks.Add(renderBreak);

            int renderBreakNumber = 0;


            for (int i = firstSprite; i < firstSprite + numberToDraw; i++)
            {
                Sprite spriteAtIndex = sa[i];

                #region if the Sprite is different from the last RenderBreak, break the batch


                if (renderBreaks[renderBreakNumber].DiffersFrom(spriteAtIndex))
                {
                    renderBreak =
                        new RenderBreak(2000 * vertexBufferNumForRenderBreaks + vertNumForRenderBreaks / 3, spriteAtIndex);

                    // mark where the break occured
                    renderBreaks.Add(renderBreak);
                    renderBreakNumber++;
                }
                #endregion


                vertNumForRenderBreaks += 6;

                if (vertNumForRenderBreaks == 6000)
                {
                    vertexBufferNumForRenderBreaks++;
                    vertNumForRenderBreaks = 0;
                }
            }

            float ratio = 1 / ((float)mFillVertexLogics.Count);

            // Why were we using the entire list when the entire list may not be drawn
            //int spriteCount = (int)(ratio * (sa.Count - firstSprite));
            int spriteCount = (int)(ratio * (numberToDraw));

            for (int i = 0; i < mFillVertexLogics.Count; i++)
            {
                mFillVertexLogics[i].SpriteList = sa;
                mFillVertexLogics[i].VertexLists = vertexLists;
                mFillVertexLogics[i].StartIndex = firstSprite + spriteCount * i;
                mFillVertexLogics[i].FirstSpriteInAllSimultaneousLogics = firstSprite;

                if (i == mFillVertexLogics.Count - 1)
                {
                    mFillVertexLogics[i].Count = (firstSprite + numberToDraw) - mFillVertexLogics[i].StartIndex;
                }
                else
                {
                    mFillVertexLogics[i].Count = spriteCount;
                }

            }


            for (int i = 0; i < mFillVertexLogics.Count; i++)
            //for (int i = mFillVertexLogics.Count - 1; i > -1; i--)
            {
                mFillVertexLogics[i].FillVertexList();
                //System.Threading.Thread.Sleep(100);
            }
            for (int i = 0; i < mFillVertexLogics.Count; i++)
            {
                mFillVertexLogics[i].Wait();
            }

//            // The numToDraw indicates the number of Sprites that will be put on the
//            // VertexBuffer, but it does not correspond to the range of Sprites that are
//            // to be traversed on the array.
//            // It is possible and likely that some of the Sprites in the sa will be invisible.
//            // When one of these invisible Sprites is encountered, skip over it, but increase the
//            // range by increasing numToDraw.  It's IMPORTANT that numToDraw is not used anywhere else
//            // besides the check for the if statement
//            int count = mCompressedSpriteList.Count;

//            for(int i = 0; i < count; i++)
//            {
//                vertNum = (i * 6) % 6000;
//                vertexBufferNum = i / 1000;
//                arrayAtIndex = vertexLists[vertexBufferNum];

//                Sprite spriteAtIndex = mCompressedSpriteList[i];

//                #region the Sprite doesn't have stored vertices (default) so we have to create them now
//                if (spriteAtIndex.mAutomaticallyUpdated)
//                {
//                    // Eventually we'll want to use the actual camera being rendered so 
//                    // we can get billboarding to work properly on split screen view.
//                    spriteAtIndex.UpdateVertices(SpriteManager.Camera);


//                    #region Set the color
//#if XNA4 

//                    arrayAtIndex[vertNum + 0].Color.PackedValue =
//                        ((uint)(255 * spriteAtIndex.mVertices[3].Color.X)) +
//                        (((uint)(255 * spriteAtIndex.mVertices[3].Color.Y)) << 8) +
//                        (((uint)(255 * spriteAtIndex.mVertices[3].Color.Z)) << 16) +
//                        (((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) << 24);
//                    arrayAtIndex[vertNum + 1].Color.PackedValue =
//                        arrayAtIndex[vertNum + 0].Color.PackedValue;

//                    arrayAtIndex[vertNum + 2].Color.PackedValue =
//                        arrayAtIndex[vertNum + 0].Color.PackedValue;

//                    arrayAtIndex[vertNum + 5].Color.PackedValue =
//                        arrayAtIndex[vertNum + 0].Color.PackedValue;
//#elif WINDOWS_8

//                    // If the Sprite's Texture is null, it will behave as if it's got its ColorOperation set to Color instead of Texture
//                    if (spriteAtIndex.ColorOperation == FlatRedBall.Graphics.ColorOperation.Texture && spriteAtIndex.Texture != null)
//                    {
//                        // If we are using the texture color, we want to ignore the Sprite's RGB values.  The W component is Alpha, so 
//                        // we'll use full values for the others.
//                        //arrayAtIndex[vertNum + 0].Color.PackedValue =
//                        //    ((uint)(255)) +
//                        //    (((uint)(255)) << 8) +
//                        //    (((uint)(255)) << 16) +
//                        //    (((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) << 24);
//                        arrayAtIndex[vertNum + 0].Color.PackedValue =
//    ((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) +
//    (((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) << 8) +
//    (((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) << 16) +
//    (((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) << 24);
//                    }
//                    else
//                    {
//                        // If we are using the texture color, we 
//                        arrayAtIndex[vertNum + 0].Color.PackedValue =
//                            ((uint)(255 * spriteAtIndex.mVertices[3].Color.X)) +
//                            (((uint)(255 * spriteAtIndex.mVertices[3].Color.Y)) << 8) +
//                            (((uint)(255 * spriteAtIndex.mVertices[3].Color.Z)) << 16) +
//                            (((uint)(255 * spriteAtIndex.mVertices[3].Color.W)) << 24);
//                    }
//                    arrayAtIndex[vertNum + 1].Color.PackedValue =
//                        arrayAtIndex[vertNum + 0].Color.PackedValue;

//                    arrayAtIndex[vertNum + 2].Color.PackedValue =
//                        arrayAtIndex[vertNum + 0].Color.PackedValue;

//                    arrayAtIndex[vertNum + 5].Color.PackedValue =
//                        arrayAtIndex[vertNum + 0].Color.PackedValue;

//#else

//                    vertexLists[vertexBufferNum][vertNum + 0].Color.PackedValue =
//                        ((uint)(255 * sa[i].mVertices[3].Color.Z)) +
//                        (((uint)(255 * sa[i].mVertices[3].Color.Y)) << 8) +
//                        (((uint)(255 * sa[i].mVertices[3].Color.X)) << 16) +
//                        (((uint)(255 * sa[i].mVertices[3].Color.W)) << 24);
//                    vertexLists[vertexBufferNum][vertNum + 1].Color.PackedValue =
//                        ((uint)(255 * sa[i].mVertices[0].Color.Z)) +
//                        (((uint)(255 * sa[i].mVertices[0].Color.Y)) << 8) +
//                        (((uint)(255 * sa[i].mVertices[0].Color.X)) << 16) +
//                        (((uint)(255 * sa[i].mVertices[0].Color.W)) << 24);
//                    vertexLists[vertexBufferNum][vertNum + 2].Color.PackedValue =
//                        ((uint)(255 * sa[i].mVertices[1].Color.Z)) +
//                        (((uint)(255 * sa[i].mVertices[1].Color.Y)) << 8) +
//                        (((uint)(255 * sa[i].mVertices[1].Color.X)) << 16) +
//                        (((uint)(255 * sa[i].mVertices[1].Color.W)) << 24);
//                    vertexLists[vertexBufferNum][vertNum + 5].Color.PackedValue =
//                        ((uint)(255 * sa[i].mVertices[2].Color.Z)) +
//                        (((uint)(255 * sa[i].mVertices[2].Color.Y)) << 8) +
//                        (((uint)(255 * sa[i].mVertices[2].Color.X)) << 16) +
//                        (((uint)(255 * sa[i].mVertices[2].Color.W)) << 24);
//#endif
//                    #endregion


//                    arrayAtIndex[vertNum + 0].Position = spriteAtIndex.mVertices[3].Position;
//                    arrayAtIndex[vertNum + 0].TextureCoordinate = spriteAtIndex.mVertices[3].TextureCoordinate;


//                    arrayAtIndex[vertNum + 1].Position = spriteAtIndex.mVertices[0].Position;
//                    arrayAtIndex[vertNum + 1].TextureCoordinate = spriteAtIndex.mVertices[0].TextureCoordinate;

//                    arrayAtIndex[vertNum + 2].Position = spriteAtIndex.mVertices[1].Position;
//                    arrayAtIndex[vertNum + 2].TextureCoordinate = spriteAtIndex.mVertices[1].TextureCoordinate;

//                    arrayAtIndex[vertNum + 3] = arrayAtIndex[vertNum + 0];
//                    arrayAtIndex[vertNum + 4] = arrayAtIndex[vertNum + 2];

//                    arrayAtIndex[vertNum + 5].Position = spriteAtIndex.mVertices[2].Position;
//                    arrayAtIndex[vertNum + 5].TextureCoordinate = spriteAtIndex.mVertices[2].TextureCoordinate;

//                    if (spriteAtIndex.FlipHorizontal)
//                    {
//                        arrayAtIndex[vertNum + 0].TextureCoordinate = arrayAtIndex[vertNum + 5].TextureCoordinate;
//                        arrayAtIndex[vertNum + 5].TextureCoordinate = arrayAtIndex[vertNum + 3].TextureCoordinate;
//                        arrayAtIndex[vertNum + 3].TextureCoordinate = arrayAtIndex[vertNum + 0].TextureCoordinate;

//                        arrayAtIndex[vertNum + 2].TextureCoordinate = arrayAtIndex[vertNum + 1].TextureCoordinate;
//                        arrayAtIndex[vertNum + 1].TextureCoordinate = arrayAtIndex[vertNum + 4].TextureCoordinate;
//                        arrayAtIndex[vertNum + 4].TextureCoordinate = arrayAtIndex[vertNum + 2].TextureCoordinate;
//                    }
//                    if (spriteAtIndex.FlipVertical)
//                    {
//                        arrayAtIndex[vertNum + 0].TextureCoordinate = arrayAtIndex[vertNum + 1].TextureCoordinate;
//                        arrayAtIndex[vertNum + 1].TextureCoordinate = arrayAtIndex[vertNum + 3].TextureCoordinate;
//                        arrayAtIndex[vertNum + 3].TextureCoordinate = arrayAtIndex[vertNum + 0].TextureCoordinate;

//                        arrayAtIndex[vertNum + 2].TextureCoordinate = arrayAtIndex[vertNum + 5].TextureCoordinate;
//                        arrayAtIndex[vertNum + 5].TextureCoordinate = arrayAtIndex[vertNum + 4].TextureCoordinate;
//                        arrayAtIndex[vertNum + 4].TextureCoordinate = arrayAtIndex[vertNum + 2].TextureCoordinate;
//                    }
//                }
//                #endregion
//                else
//                {
//                    arrayAtIndex[vertNum + 0] = spriteAtIndex.mVerticesForDrawing[3];
//                    arrayAtIndex[vertNum + 1] = spriteAtIndex.mVerticesForDrawing[0];
//                    arrayAtIndex[vertNum + 2] = spriteAtIndex.mVerticesForDrawing[1];
//                    arrayAtIndex[vertNum + 3] = spriteAtIndex.mVerticesForDrawing[3];
//                    arrayAtIndex[vertNum + 4] = spriteAtIndex.mVerticesForDrawing[1];
//                    arrayAtIndex[vertNum + 5] = spriteAtIndex.mVerticesForDrawing[2];
//                }

//            }

        }

        private static int FillVBList(List<Text> texts,
            List<DynamicVertexBuffer> vertexBufferList, Camera camera,
            List<RenderBreak> renderBreaks, int firstText,
            int numToDraw, int numberOfVertices)
        {
            #region if the array is empty or if all texts are empty, just exit
            bool toExit = true;

            if (texts.Count != 0)
            {
                for (int i = 0; i < texts.Count; i++)
                {
                    Text t = texts[i];
                    if (t.VertexCount != 0)
                    {
                        toExit = false;
                        break;
                    }
                }
            }

            if (toExit)
                return 0;
            #endregion

            renderBreaks.Clear();
            int vertexBufferNum = 0;

            #region grab the first vertex buffer
            DynamicVertexBuffer vb = vertexBufferList[vertexBufferNum];

            //      VertexPositionColorTexture[] vertArray = new VertexPositionColorTexture[System.Math.Min(6000, numberOfVertices)];

            int vertNum = 0;
            #endregion

            RenderBreak renderBreak = new RenderBreak(firstText, texts[firstText].Font.Texture, texts[firstText].ColorOperation,
                BlendOperation.Regular, TextureAddressMode.Clamp);

#if DEBUG
            renderBreak.ObjectCausingBreak = texts[firstText];
#endif

            renderBreaks.Add(renderBreak);

            int renderBreakNumber = 0;

            #region Calculate verticesLeftToRender
            int verticesLeftToRender = 0;

            for (int i = firstText; i < firstText + numToDraw; i++)
            {
                if (texts[i].AbsoluteVisible)
                    verticesLeftToRender += texts[i].VertexCount;
            }
            #endregion

            for (int i = firstText; i < firstText + numToDraw; i++)
            {
                #region if the Text is different from the last RenderBreak, break the batch
                if (renderBreaks[renderBreakNumber].DiffersFrom(texts[i]))
                {
                    renderBreak = new RenderBreak((2000 * vertexBufferNum + vertNum / 3),
                        texts[i].Font.Texture, texts[i].ColorOperation, texts[i].BlendOperation, TextureAddressMode.Clamp);
#if DEBUG
                        renderBreak.ObjectCausingBreak = texts[i];
#endif
                    renderBreaks.Add(renderBreak);
                    renderBreakNumber++;
                }
                #endregion

                int verticesLeftOnThisText = texts[i].VertexCount;

                #region If this text will fit on the current vertex buffer, copy over the info
                if (vertNum + verticesLeftOnThisText < 6000)
                {
                    for (int textVertex = 0; textVertex < texts[i].VertexCount; textVertex++)
                    {
                        mVertexArray[vertNum] = texts[i].VertexArray[textVertex];
                        vertNum++;
                    }
                    verticesLeftToRender -= texts[i].VertexCount;

                    if (vertNum == 6000 &&
                        ((i + 1 < firstText + numToDraw) ||
                         verticesLeftToRender != 0))
                    {
#if MONODROID
                        vertexBufferList[vertexBufferNum].SetData<VertexPositionColorTexture>(mVertexArray);
#else
                        vertexBufferList[vertexBufferNum].SetData<VertexPositionColorTexture>(mVertexArray, 0, vertNum, SetDataOptions.Discard);
#endif
                        vertexBufferNum++;
                        vertNum = 0;
                    }
                }
                #endregion

                #region The text won't fit on this vertex buffer, so copy some over so break the text up
                else
                {
                    int textVertexIndexOn = 0;
                    while (verticesLeftOnThisText > 0)
                    {
                        int numberToCopy = System.Math.Min(verticesLeftToRender, 6000 - vertNum);

                        for (int numberCopied = 0; numberCopied < numberToCopy; numberCopied++)
                        {
                            mVertexArray[vertNum] = texts[i].VertexArray[textVertexIndexOn];
                            vertNum++;
                            verticesLeftToRender--;
                            textVertexIndexOn++;
                        }

                        // now write the verts if there is more to draw after this
                        if (vertNum == 6000 &&
                            ((i + 1 < firstText + numToDraw) ||
                             verticesLeftToRender != 0))
                        {
#if MONODROID
                            vertexBufferList[vertexBufferNum].SetData<VertexPositionColorTexture>(mVertexArray);
#else
                            vertexBufferList[vertexBufferNum].SetData<VertexPositionColorTexture>(mVertexArray, 0, vertNum, SetDataOptions.Discard);
#endif
                            vertexBufferNum++;
                            vertNum = 0;
                            verticesLeftOnThisText -= 6000;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                #endregion
            }

            // This is slow on the 360.  I have no idea why, and I'm actually suspicious of the time reports. --Vic
#if XNA4 || MONOGAME
            GraphicsDevice.SetVertexBuffer(null);
#else
            GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
#endif


#if XBOX360
            vertexBufferList[vertexBufferNum].SetData<VertexPositionColorTexture>(mVertexArray, 0, vertNum);
#else
#if MONODROID
            vertexBufferList[vertexBufferNum].SetData<VertexPositionColorTexture>(mVertexArray);
#else
            vertexBufferList[vertexBufferNum].SetData<VertexPositionColorTexture>(mVertexArray, 0, vertNum, SetDataOptions.Discard);
#endif
#endif

            return vertNum / 3 + vertexBufferNum * 2000;
        }

        private static int FillVertexList(IList<Text> texts,
            List<VertexPositionColorTexture[]> vertexLists, Camera camera,
            List<RenderBreak> renderBreaks, int firstText,
            int numToDraw, int numberOfVertices)
        {
            #region if the array is empty or if all texts are empty, just exit
            bool toExit = true;

            if (texts.Count != 0)
            {
                for (int i = 0; i < texts.Count; i++)
                {
                    Text t = texts[i];
                    if (t.AbsoluteVisible && t.VertexCount != 0)
                    {
                        toExit = false;
                        break;
                    }
                }
            }

            if (toExit)
                return 0;
            #endregion

            renderBreaks.Clear();
            int vertexBufferNum = 0;

            #region grab the first vertex buffer
            //DynamicVertexBuffer vb = vertexBufferList[vertexBufferNum];

            //      VertexPositionColorTexture[] vertArray = new VertexPositionColorTexture[System.Math.Min(6000, numberOfVertices)];

            int vertNum = 0;
            #endregion

            RenderBreak renderBreak = new RenderBreak(firstText, texts[firstText], 0);

            renderBreaks.Add(renderBreak);

            int renderBreakNumber = 0;

            #region Calculate verticesLeftToRender
            int verticesLeftToRender = 0;

            for (int i = firstText; i < firstText + numToDraw; i++)
            {
                if (texts[i].AbsoluteVisible)
                    verticesLeftToRender += texts[i].VertexCount;
            }
            #endregion

            for (int i = firstText; i < firstText + numToDraw; i++)
            {
                Text textAtIndex = texts[i];

                if (textAtIndex.AbsoluteVisible == false || textAtIndex.VertexCount == 0)
                    continue;

                #region if the Text is different from the last RenderBreak, break the batch
                if (renderBreaks[renderBreakNumber].DiffersFrom(textAtIndex))
                {
                    renderBreak = new RenderBreak((2000 * vertexBufferNum + vertNum / 3),
                        textAtIndex, 0);

                    renderBreaks.Add(renderBreak);
                    renderBreakNumber++;
                }
                #endregion

                int verticesLeftOnThisText = textAtIndex.VertexCount;

                #region If this text will fit on the current vertex buffer, copy over the info
                if (vertNum + verticesLeftOnThisText < 6000)
                {
                    Array.Copy(textAtIndex.VertexArray, 0, vertexLists[vertexBufferNum], vertNum, textAtIndex.VertexCount);

                    AddRenderBreaksForTextureSwitches(textAtIndex, renderBreaks, ref renderBreakNumber, vertNum / 3, 0, int.MaxValue);

                    vertNum += textAtIndex.VertexCount;

                    verticesLeftToRender -= textAtIndex.VertexCount;


                    if (vertNum == 6000 &&
                        ((i + 1 < firstText + numToDraw) ||
                         verticesLeftToRender != 0))
                    {
                        //vertexBufferList[vertexBufferNum].SetData<VertexPositionColorTexture>(mVertexArray, 0, vertNum, SetDataOptions.Discard);
                        vertexBufferNum++;
                        vertNum = 0;
                    }
                }
                #endregion

                #region The text won't fit on this vertex buffer, so copy some over so break the text up
                else
                {
                    int textVertexIndexOn = 0;
                    while (verticesLeftOnThisText > 0)
                    {
                        int numberToCopy = System.Math.Min(verticesLeftOnThisText, 6000 - vertNum);

                        int relativeIndexToStartAt = textAtIndex.VertexCount - verticesLeftOnThisText;

                        AddRenderBreaksForTextureSwitches(textAtIndex, renderBreaks, ref renderBreakNumber, vertNum / 3,
                            relativeIndexToStartAt / 3, numberToCopy / 3 + relativeIndexToStartAt);

                        // This can be sped up using Array.Copy.  Do this sometime.
                        for (int numberCopied = 0; numberCopied < numberToCopy; numberCopied++)
                        {
                            vertexLists[vertexBufferNum][vertNum] = texts[i].VertexArray[textVertexIndexOn];
                            vertNum++;
                            verticesLeftToRender--;
                            textVertexIndexOn++;
                        }

                        // now write the verts if there is more to draw after this
                        if (vertNum == 6000 &&
                            ((i + 1 < firstText + numToDraw) ||
                             verticesLeftToRender != 0))
                        {
                            // vertexBufferList[vertexBufferNum].SetData<VertexPositionColorTexture>(mVertexArray, 0, vertNum, SetDataOptions.Discard);

                            vertexBufferNum++;
                            vertNum = 0;
                            verticesLeftOnThisText -= textVertexIndexOn;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                #endregion
            }

            // This is slow on the 360.  I have no idea why, and I'm actually suspicious of the time reports. --Vic
            //            GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
            //#if XBOX360
            //            vertexBufferList[vertexBufferNum].SetData<VertexPositionColorTexture>(mVertexArray);
            //#else
            //            vertexBufferList[vertexBufferNum].SetData<VertexPositionColorTexture>(mVertexArray, 0, vertNum, SetDataOptions.Discard);
            //#endif

            return vertNum / 3 + vertexBufferNum * 2000;
        }

        private static void AddRenderBreaksForTextureSwitches(Text textAtIndex,
            List<RenderBreak> renderBreaks, ref int renderBreakNumber, int startIndex, int minimumTriangleIndex, int maximumTriangleIndex)
        {
            for (int i = 0; i < textAtIndex.mInternalTextureSwitches.Count; i++)
            {

                Microsoft.Xna.Framework.Point point = textAtIndex.mInternalTextureSwitches[i];

                if (point.X >= minimumTriangleIndex && point.X < maximumTriangleIndex)
                {

                    RenderBreak renderBreakToAdd = new RenderBreak(
                        startIndex + point.X,
                        textAtIndex, point.Y);

                    renderBreaks.Add(renderBreakToAdd);

                    renderBreakNumber++;
                }
            }
        }

        #endregion


        public static void SetSharedEffectParameters(Camera camera)
        {
#if !XNA4 && !MONOGAME
            SetSharedEffectParameters(camera, mModelEffect, mModelEffectParameterCache);
#endif
        }

#if !XNA4 && !MONOGAME

        public static void SetSharedEffectParameters(Camera camera, Effect effect, EffectCache effectCache)
        {
            // Set all shared variables
            List<EffectParameter> paramList;

        #region View
            paramList = effectCache[EffectCache.EffectParameterNamesEnum.View];
            if (paramList != null)
            {
                foreach (EffectParameter param in paramList)
                {
                    param.SetValue(camera.View);
                }
            }
        #endregion
        #region Projection
            paramList = effectCache[EffectCache.EffectParameterNamesEnum.Projection];
            if (paramList != null)
            {
                foreach (EffectParameter param in paramList)
                {
                    param.SetValue(camera.Projection);
                }
            }
        #endregion
        #region ViewProj
            paramList = effectCache[EffectCache.EffectParameterNamesEnum.ViewProj];
            if (paramList != null)
            {
                foreach (EffectParameter param in paramList)
                {
                    param.SetValue(camera.ViewProjection);
                }
            }
        #endregion

        #region PixelSize
            paramList = effectCache[EffectCache.EffectParameterNamesEnum.PixelSize];
            if (paramList != null)
            {
                foreach (EffectParameter param in paramList)
                {
                    param.SetValue(new Vector2(
                        1f / (float)FlatRedBallServices.ClientWidth,
                        1f / (float)FlatRedBallServices.ClientHeight));
                }
            }
        #endregion
        #region ViewportSize
            paramList = effectCache[EffectCache.EffectParameterNamesEnum.ViewportSize];
            if (paramList != null)
            {
                foreach (EffectParameter param in paramList)
                {
                    param.SetValue(new Vector2(
                        (float)camera.DestinationRectangle.Width, (float)camera.DestinationRectangle.Height));
                }
            }
        #endregion
        #region InvViewProj
            paramList = effectCache[EffectCache.EffectParameterNamesEnum.InvViewProj];
            if (paramList != null)
            {
                Matrix invViewProj;
                Matrix viewProj = camera.ViewProjection;
                Matrix.Invert(ref viewProj, out invViewProj);
                
                foreach (EffectParameter param in paramList)
                {
                    param.SetValue(invViewProj);
                }
            }
        #endregion
        #region NearClipPlane
            paramList = effectCache[EffectCache.EffectParameterNamesEnum.NearClipPlane];
            if (paramList != null)
            {
                foreach (EffectParameter param in paramList)
                {
                    param.SetValue(camera.NearClipPlane);
                }
            }
        #endregion
        #region FarClipPlane
            paramList = effectCache[EffectCache.EffectParameterNamesEnum.FarClipPlane];
            if (paramList != null)
            {
                foreach (EffectParameter param in paramList)
                {
                    param.SetValue(camera.FarClipPlane);
                }
            }
        #endregion
        #region CameraPosition
            paramList = effectCache[EffectCache.EffectParameterNamesEnum.CameraPosition];
            if (paramList != null)
            {
                foreach (EffectParameter param in paramList)
                {
                    param.SetValue(camera.Position);
                }
            }
        #endregion

        #region Shadows
            if (camera.MyShadow != null) camera.MyShadow.SetupEffectValues(effectCache);
        #endregion


            // Commit Changes
            effect.CommitChanges();

            //SetSharedEffectParameters(camera, mModelEffect);
        }

#endif

        public static void SetSharedEffectParameters(Camera camera, Effect effect)
        {
#if !WINDOWS_PHONE && !MONOGAME
            // Set effect variables
            if (effect.Parameters["PixelSize"] != null)
                effect.Parameters["PixelSize"].SetValue(new Vector2(
                    1f / (float)FlatRedBallServices.ClientWidth,
                    1f / (float)FlatRedBallServices.ClientHeight));
            if (effect.Parameters["ViewportSize"] != null)
                effect.Parameters["ViewportSize"].SetValue(new Vector2(
                    (float)camera.DestinationRectangle.Width, (float)camera.DestinationRectangle.Height));
            if (effect.Parameters["InvViewProj"] != null)
                effect.Parameters["InvViewProj"].SetValue(
                    Matrix.Invert(
                        camera.GetLookAtMatrix() *
                        camera.GetProjectionMatrix()
                        )
                    );
            if (effect.Parameters["NearClipPlane"] != null)
                effect.Parameters["NearClipPlane"].SetValue(camera.NearClipPlane);
            if (effect.Parameters["FarClipPlane"] != null)
                effect.Parameters["FarClipPlane"].SetValue(camera.FarClipPlane);
            if (effect.Parameters["CameraPosition"] != null)
                effect.Parameters["CameraPosition"].SetValue(camera.Position);
#endif
        }

        private static void SortBatchesZInsertionAscending(IList<IDrawableBatch> batches)
        {
            if (batches.Count == 1 || batches.Count == 0)
                return;

            int whereBatchBelongs;

            for (int i = 1; i < batches.Count; i++)
            {
                if ((batches[i]).Z < (batches[i - 1]).Z)
                {
                    if (i == 1)
                    {
                        batches.Insert(0, batches[i]);
                        batches.RemoveAt(i + 1);
                        continue;
                    }

                    for (whereBatchBelongs = i - 2; whereBatchBelongs > -1; whereBatchBelongs--)
                    {
                        if ((batches[i]).Z >= (batches[whereBatchBelongs]).Z)
                        {
                            batches.Insert(whereBatchBelongs + 1, batches[i]);
                            batches.RemoveAt(i + 1);
                            break;
                        }
                        else if (whereBatchBelongs == 0 && (batches[i]).Z < (batches[0]).Z)
                        {
                            batches.Insert(0, batches[i]);
                            batches.RemoveAt(i + 1);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}