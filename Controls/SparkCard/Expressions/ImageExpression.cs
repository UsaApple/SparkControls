using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.IO;

using SparkControls.Expression;
using SparkControls.Foundation;
using SparkControls.Public;

namespace SparkControls.Controls
{
    /// <summary>
    /// 表示图片、表达式的组合类型。
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ImageExpressionConverter))]
    public class ImageExpression : BaseExpression<Image>
    {
        // 图片缓存
        private static readonly Dictionary<string, Image> _images = new Dictionary<string, Image>();

        /// <summary>
        /// 初始 ImageExpression 类型的新实例。
        /// </summary>
        public ImageExpression()
        {

        }

        /// <summary>
        /// 初始 ImageExpression 类型的新实例。
        /// </summary>
        /// <param name="value">固定值 - 图片。</param>
        /// <param name="file">固定值 - 文件。</param>
        /// <param name="expression">表达式。</param>
        public ImageExpression(Image value, string file, string expression)
        {
            Value = value;
            File = file;
            Expression = expression;
        }

        /// <summary>
        /// 固定值 - 图片
        /// </summary>
        [Description("用于生成结果的固定值，优先级低于“File”和“Expression”属性。")]
        [Editor("System.Drawing.Design.ImageEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [TypeConverter(typeof(StringConverter))]
        public Image Value { get; set; }

        /// <summary>
        /// 固定值 - 文件
        /// </summary>
        [Description("用于生成结果的固定值，优先级高于“Value”属性。")]
        [TypeConverter(typeof(StringConverter))]
        public string File { get; set; }

        /// <summary>
        /// 表达式
        /// </summary>
        [Description("用于生成结果的表达式，优先级高于“File”属性。")]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [TypeConverter(typeof(StringConverter))]
        public string Expression { get; set; }

        /// <summary>
        /// 计算表达式的值。
        /// </summary>
        /// <param name="dataSource">表示数据源的 DataRowView 实例。</param>
        /// <returns>表达式的计算值。</returns>
        public override Image GetValue(DataRowView dataSource = null)
        {
            Image GetImageFromFile(string filePath, string fileName)
            {
                if (fileName.IsNullOrEmpty()) return null;
                //Function

                if (_images.ContainsKey(fileName))
                {
                    return _images[fileName];
                }
                else if (_images.ContainsKey(Path.Combine(filePath, "Resources", fileName).ToLowerInvariant()))
                {
                    return _images[Path.Combine(filePath, "Resources", fileName).ToLowerInvariant()];
                }
                else if (_images.ContainsKey(Path.Combine(filePath, "Resources", fileName + ".png").ToLowerInvariant()))
                {
                    return _images[Path.Combine(filePath, "Resources", fileName + ".png").ToLowerInvariant()];
                }

                var enumImage = fileName.ToEnum<EnumImageList>(EnumImageList.None);
                if (enumImage != EnumImageList.None)
                {
                    var image = Function.GetImage(enumImage);
                    _images.TryAdd(fileName, image);
                    return image;
                }
                else
                {
                    string path = Path.Combine(filePath, "Resources", fileName);
                    if (System.IO.File.Exists(path))
                    {
                        var image = Image.FromFile(path);
                        _images.TryAdd(path.ToLowerInvariant(), image);
                        return image;
                    }
                    else
                    {
                        path = Path.Combine(filePath, "Resources", fileName + ".png");
                        if (System.IO.File.Exists(path))
                        {
                            var image = Image.FromFile(path);
                            _images.TryAdd(path.ToLowerInvariant(), image);
                            return image;
                        }
                    }
                }
                return null;
            }

            if (Expression.IsNullOrEmpty())
            {
                if (File.IsNullOrEmpty())
                {
                    return Value;
                }
                else
                {
                    return GetImageFromFile(AppDomain.CurrentDomain.BaseDirectory, File);
                }
            }

            try
            {
                // 数据源替换
                string expression = dataSource == null ? Expression : Converter.ReplaceSplit(Expression, dataSource.Row);
                // 表达式计算
                string fileName = new DCExpression(expression).Eval<string>();
                // 获取图片
                return GetImageFromFile(AppDomain.CurrentDomain.BaseDirectory, fileName);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}