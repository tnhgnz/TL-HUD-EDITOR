using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;

namespace TLHudEditor
{
    public partial class ConfigEditWindow : Window
    {
        private static readonly string[] AlignPresets =
        {
            "LeftTop", "CenterTop", "RightTop",
            "LeftCenter", "CenterCenter", "RightCenter",
            "LeftBottom", "CenterBottom", "RightBottom"
        };

        private readonly string configPath;
        private readonly AzulejoLayoutDocument configDocument;
        private readonly List<string> alignOptions = new(AlignPresets);

        private ComponentTransform? activeTransform;
        private bool loadingUi;
        private TransformBaseline? selectionBaseline;

        public ConfigEditWindow(string path, AzulejoLayoutDocument config)
        {
            InitializeComponent();

            configPath = path;
            configDocument = config;

            AlignCombo.ItemsSource = alignOptions;

            FramesList.ItemsSource = Utils.ComponentNames;
            FramesList.SelectedIndex = 0;
        }

        private void OnWindowClosed(object? sender, EventArgs e) =>
            Application.Current?.MainWindow?.Show();

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void FramesListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FramesList.SelectedValue is not int key)
                return;

            activeTransform = GetOrCreateTransform(key);
            selectionBaseline = TransformBaseline.CaptureFrom(activeTransform);
            PushTransformToUi(activeTransform);
        }

        private void AlignComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loadingUi || activeTransform == null)
                return;
            if (AlignCombo.SelectedItem is string s)
                activeTransform.Align = s;
        }

        private void ResetFieldButtonClick(object sender, RoutedEventArgs e)
        {
            if (selectionBaseline == null || activeTransform == null)
                return;
            if (sender is not FrameworkElement el || el.Tag is not string tag)
                return;

            var b = selectionBaseline;
            var t = activeTransform;

            loadingUi = true;
            try
            {
                switch (tag)
                {
                    case "Align":
                    {
                        var align = b.Align;
                        if (!alignOptions.Contains(align))
                            alignOptions.Add(align);
                        t.Align = align;
                        AlignCombo.SelectedItem = align;
                    }
                    break;
                    case "DesiredWidth":
                        t.DesiredSize ??= new IntSize2D();
                        var w = (int)ClampInt(b.DesiredWidth, (int)SliderDesiredWidth.Minimum, (int)SliderDesiredWidth.Maximum);
                        t.DesiredSize.Width = w;
                        SliderDesiredWidth.Value = w;
                        TxtDesiredWidth.Text = w.ToString();
                        break;
                    case "DesiredHeight":
                        t.DesiredSize ??= new IntSize2D();
                        var h = (int)ClampInt(b.DesiredHeight, (int)SliderDesiredHeight.Minimum, (int)SliderDesiredHeight.Maximum);
                        t.DesiredSize.Height = h;
                        SliderDesiredHeight.Value = h;
                        TxtDesiredHeight.Text = h.ToString();
                        break;
                    case "TranslateX":
                        t.Translate ??= new Vector3D();
                        var tx = Clamp(b.TranslateX, SliderTranslateX.Minimum, SliderTranslateX.Maximum);
                        t.Translate.X = tx;
                        SliderTranslateX.Value = tx;
                        TxtTranslateX.Text = FormatDouble(tx);
                        break;
                    case "TranslateY":
                        t.Translate ??= new Vector3D();
                        var ty = Clamp(b.TranslateY, SliderTranslateY.Minimum, SliderTranslateY.Maximum);
                        t.Translate.Y = ty;
                        SliderTranslateY.Value = ty;
                        TxtTranslateY.Text = FormatDouble(ty);
                        break;
                    case "Scale":
                        t.Scale ??= new Vector3D { X = 1, Y = 1, Z = 1 };
                        var su = Clamp(b.ScaleUniform, SliderScaleUniform.Minimum, SliderScaleUniform.Maximum);
                        t.Scale.X = su;
                        t.Scale.Y = su;
                        t.Scale.Z = su;
                        SliderScaleUniform.Value = su;
                        TxtScale.Text = FormatDouble(su);
                        break;
                    case "ZOrder":
                        var z = (int)Clamp(b.ZOrder, SliderZOrder.Minimum, SliderZOrder.Maximum);
                        t.ZOrder = z;
                        SliderZOrder.Value = z;
                        TxtZOrder.Text = z.ToString();
                        break;
                }
            }
            finally
            {
                loadingUi = false;
            }
        }

        private void NumericSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (loadingUi || activeTransform == null)
                return;

            if (sender == SliderDesiredWidth)
            {
                activeTransform.DesiredSize ??= new IntSize2D();
                activeTransform.DesiredSize.Width = (int)Math.Round(e.NewValue);
                TxtDesiredWidth.Text = activeTransform.DesiredSize.Width.ToString();
            }
            else if (sender == SliderDesiredHeight)
            {
                activeTransform.DesiredSize ??= new IntSize2D();
                activeTransform.DesiredSize.Height = (int)Math.Round(e.NewValue);
                TxtDesiredHeight.Text = activeTransform.DesiredSize.Height.ToString();
            }
            else if (sender == SliderTranslateX)
            {
                activeTransform.Translate ??= new Vector3D();
                activeTransform.Translate.X = e.NewValue;
                TxtTranslateX.Text = FormatDouble(e.NewValue);
            }
            else if (sender == SliderTranslateY)
            {
                activeTransform.Translate ??= new Vector3D();
                activeTransform.Translate.Y = e.NewValue;
                TxtTranslateY.Text = FormatDouble(e.NewValue);
            }
            else if (sender == SliderScaleUniform)
            {
                activeTransform.Scale ??= new Vector3D { X = 1, Y = 1, Z = 1 };
                var s = e.NewValue;
                activeTransform.Scale.X = s;
                activeTransform.Scale.Y = s;
                activeTransform.Scale.Z = s;
                TxtScale.Text = FormatDouble(s);
            }
            else if (sender == SliderZOrder)
            {
                activeTransform.ZOrder = (int)Math.Round(e.NewValue);
                TxtZOrder.Text = activeTransform.ZOrder.ToString();
            }
        }

        private static string FormatDouble(double v) => v.ToString("0.###");

        private static double GetUniformScale(ComponentTransform t)
        {
            t.Scale ??= new Vector3D { X = 1, Y = 1, Z = 1 };
            return t.Scale.X;
        }

        private ComponentTransform GetOrCreateTransform(int componentKey)
        {
            configDocument.Payload ??= new AzulejoPayload();
            configDocument.Payload.Transforms ??= new List<ComponentTransform>();

            var list = configDocument.Payload.Transforms;
            var existing = list.FirstOrDefault(t => t.ComponentKey == componentKey);
            if (existing != null)
                return existing;

            var created = new ComponentTransform
            {
                ComponentKey = componentKey,
                Align = "CenterCenter",
                DesiredSize = new IntSize2D { Width = 100, Height = 100 },
                Translate = new Vector3D(),
                Scale = new Vector3D { X = 1, Y = 1, Z = 1 },
                ZOrder = 0
            };
            list.Add(created);
            return created;
        }

        private void PushTransformToUi(ComponentTransform t)
        {
            loadingUi = true;
            try
            {
                var align = t.Align ?? "CenterCenter";
                if (!alignOptions.Contains(align))
                    alignOptions.Add(align);
                AlignCombo.SelectedItem = align;

                t.DesiredSize ??= new IntSize2D();
                var dw = (int)Clamp(t.DesiredSize.Width, SliderDesiredWidth.Minimum, SliderDesiredWidth.Maximum);
                var dh = (int)Clamp(t.DesiredSize.Height, SliderDesiredHeight.Minimum, SliderDesiredHeight.Maximum);
                SliderDesiredWidth.Value = dw;
                SliderDesiredHeight.Value = dh;
                TxtDesiredWidth.Text = dw.ToString();
                TxtDesiredHeight.Text = dh.ToString();

                t.Translate ??= new Vector3D();
                var tx = Clamp(t.Translate.X, SliderTranslateX.Minimum, SliderTranslateX.Maximum);
                var ty = Clamp(t.Translate.Y, SliderTranslateY.Minimum, SliderTranslateY.Maximum);
                SliderTranslateX.Value = tx;
                SliderTranslateY.Value = ty;
                TxtTranslateX.Text = FormatDouble(tx);
                TxtTranslateY.Text = FormatDouble(ty);

                t.Scale ??= new Vector3D { X = 1, Y = 1, Z = 1 };
                var s = Clamp(GetUniformScale(t), SliderScaleUniform.Minimum, SliderScaleUniform.Maximum);
                SliderScaleUniform.Value = s;
                TxtScale.Text = FormatDouble(s);

                var zOrder = (int)Clamp(t.ZOrder, SliderZOrder.Minimum, SliderZOrder.Maximum);
                SliderZOrder.Value = zOrder;
                TxtZOrder.Text = zOrder.ToString();
            }
            finally
            {
                loadingUi = false;
            }

            FlushSliderClampsToTransform(t);
        }

        private static int ClampInt(int v, int min, int max) => v < min ? min : (v > max ? max : v);

        private static double Clamp(double v, double min, double max) =>
            v < min ? min : (v > max ? max : v);

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (activeTransform != null && !loadingUi)
                    FlushSliderClampsToTransform(activeTransform);

                var json = JsonConvert.SerializeObject(configDocument, Formatting.Indented);
                File.WriteAllText(configPath, json);
                MessageBox.Show("Сохранено.", "TL HUD Editor", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FlushSliderClampsToTransform(ComponentTransform t)
        {
            t.DesiredSize ??= new IntSize2D();
            t.DesiredSize.Width = (int)Math.Round(SliderDesiredWidth.Value);
            t.DesiredSize.Height = (int)Math.Round(SliderDesiredHeight.Value);

            t.Translate ??= new Vector3D();
            t.Translate.X = SliderTranslateX.Value;
            t.Translate.Y = SliderTranslateY.Value;

            t.Scale ??= new Vector3D { X = 1, Y = 1, Z = 1 };
            var su = SliderScaleUniform.Value;
            t.Scale.X = su;
            t.Scale.Y = su;
            t.Scale.Z = su;

            t.ZOrder = (int)Math.Round(SliderZOrder.Value);
            if (AlignCombo.SelectedItem is string a)
                t.Align = a;
        }

        private sealed class TransformBaseline
        {
            public int DesiredWidth;
            public int DesiredHeight;
            public string Align;
            public double TranslateX, TranslateY;
            public double ScaleUniform;
            public int ZOrder;

            public static TransformBaseline CaptureFrom(ComponentTransform t)
            {
                t.DesiredSize ??= new IntSize2D();
                t.Translate ??= new Vector3D();
                t.Scale ??= new Vector3D { X = 1, Y = 1, Z = 1 };
                return new TransformBaseline
                {
                    DesiredWidth = t.DesiredSize.Width,
                    DesiredHeight = t.DesiredSize.Height,
                    Align = t.Align ?? "CenterCenter",
                    TranslateX = t.Translate.X,
                    TranslateY = t.Translate.Y,
                    ScaleUniform = GetUniformScale(t),
                    ZOrder = t.ZOrder
                };
            }
        }
    }
}
