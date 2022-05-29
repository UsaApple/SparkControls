using System.Drawing;

using SparkControls.Controls;

namespace SparkControls
{
	/// <summary>
	/// 主题常量值
	/// </summary>
	public static class SparkThemeConstsTestBak
	{
		//默认颜色不能设置为16进制，必须设置为RGB或者颜色英文单词
		#region Base
		internal static readonly Color BackColor = ColorTranslator.FromHtml(BackColorString);
		internal static readonly Color BorderColor = ColorTranslator.FromHtml(BorderColorString);
		internal static readonly Color ForeColor = ColorTranslator.FromHtml(ForeColorString);

		internal const string BackColorString = "White";
		internal const string BorderColorString = "214,214,214";
		internal const string ForeColorString = "Black";
		#endregion

		#region Form
		internal static readonly Color FormBorderColor = ColorTranslator.FromHtml(FormBorderColorString);
		internal const string FormBorderColorString = "1,79,114";//1,79,114
		#endregion

		#region Edit
		internal static readonly Color MouseOverBackColor = ColorTranslator.FromHtml(MouseOverBackColorString);
		internal static readonly Color MouseDownBackColor = ColorTranslator.FromHtml(MouseDownBackColorString);
		internal static readonly Color SelectedBackColor = ColorTranslator.FromHtml(SelectedBackColorString);

		internal static readonly Color MouseOverBorderColor = ColorTranslator.FromHtml(MouseOverBorderColorString);
		internal static readonly Color MouseDownBorderColor = ColorTranslator.FromHtml(MouseDownBorderColorString);
		internal static readonly Color SelectedBorderColor = ColorTranslator.FromHtml(SelectedBorderColorString);

		internal static readonly Color MouseOverForeColor = ColorTranslator.FromHtml(MouseOverForeColorString);
		internal static readonly Color MouseDownForeColor = ColorTranslator.FromHtml(MouseDownForeColorString);
		internal static readonly Color SelectedForeColor = ColorTranslator.FromHtml(SelectedForeColorString);

		internal static readonly Color DisabledBackColor = ColorTranslator.FromHtml(DisabledBackColorString);

		internal const string MouseOverBackColorString = "223,223,223";
		internal const string MouseDownBackColorString = "21,163,197";//21,163,197
		internal const string SelectedBackColorString = "1,79,114";

		internal const string MouseOverBorderColorString = "223,223,223";
		internal const string MouseDownBorderColorString = "21,163,197";
		internal const string SelectedBorderColorString = "1,79,114";

		internal const string MouseOverForeColorString = "Black";
		internal const string MouseDownForeColorString = "White";
		internal const string SelectedForeColorString = "White";

		internal const string DisabledBackColorString = "222,222,222";
		#endregion

		#region Label
		internal static readonly Color LabelBackColor = ColorTranslator.FromHtml(LabelBackColorString);
		internal const string LabelBackColorString = "Transparent";
		#endregion

		#region TextBox
		internal static readonly Color TextBoxMouseOverBorderColor = ColorTranslator.FromHtml(TextBoxMouseOverBorderColorString);
		internal static readonly Color TextBoxMouseDownBorderColor = ColorTranslator.FromHtml(TextBoxMouseDownBorderColorString);
		internal static readonly Color TextBoxMouseOverBackColor = ColorTranslator.FromHtml(TextBoxMouseOverBackColorString);
		internal static readonly Color TextBoxValidateFailedBorderColor = ColorTranslator.FromHtml(TextBoxValidateFailedBorderColorString);

		internal const string TextBoxMouseOverBorderColorString = "1,79,114";
		internal const string TextBoxMouseDownBorderColorString = "1,79,114";
		internal const string TextBoxMouseOverBackColorString = "White";
		internal const string TextBoxValidateFailedBorderColorString = "Red";
		#endregion

		#region Button
		internal static readonly Color ButtonMouseOverBackColor = ColorTranslator.FromHtml(ButtonMouseOverBackColorString);
		internal static readonly Color ButtonMouseDownBackColor = ColorTranslator.FromHtml(ButtonMouseDownBackColorString);
		internal static readonly Color ButtonSelectedBackColor = ColorTranslator.FromHtml(ButtonSelectedBackColorString);

		internal static readonly Color ButtonMouseOverBorderColor = ColorTranslator.FromHtml(ButtonMouseOverBorderColorString);
		internal static readonly Color ButtonMouseDownBorderColor = ColorTranslator.FromHtml(ButtonMouseDownBorderColorString);
		internal static readonly Color ButtonSelectedBorderColor = ColorTranslator.FromHtml(ButtonSelectedBorderColorString);

		internal static readonly Color ButtonMouseOverForeColor = ColorTranslator.FromHtml(ButtonMouseOverForeColorString);
		internal static readonly Color ButtonMouseDownForeColor = ColorTranslator.FromHtml(ButtonMouseDownForeColorString);
		internal static readonly Color ButtonSelectedForeColor = ColorTranslator.FromHtml(ButtonSelectedForeColorString);

		internal static readonly Color ButtonDisabledBackColor = ColorTranslator.FromHtml(ButtonDisabledBackColorString);

		internal const string ButtonMouseOverBackColorString = "White";
		internal const string ButtonMouseDownBackColorString = "1,79,114";
		internal const string ButtonSelectedBackColorString = "1,79,114";

		internal const string ButtonMouseOverBorderColorString = "1,79,114";
		internal const string ButtonMouseDownBorderColorString = "1,79,114";
		internal const string ButtonSelectedBorderColorString = "1,79,114";

		internal const string ButtonMouseOverForeColorString = "1,79,114";
		internal const string ButtonMouseDownForeColorString = "White";
		internal const string ButtonSelectedForeColorString = "White";

		internal const string ButtonDisabledBackColorString = "245,245,245";

		#endregion

		#region CheckBox
		internal static readonly Color CheckBoxTickColor = ColorTranslator.FromHtml(CheckBoxTickColorString);
		internal static readonly Color CheckBoxIndeterminateBackColor = ColorTranslator.FromHtml(CheckBoxIndeterminateBackColorString);
		internal static readonly Color CheckBoxFocusedBorderColor = ColorTranslator.FromHtml(CheckBoxFocusedBorderColorString);

		internal static readonly Color CheckBoxMouseOverBackColor = ColorTranslator.FromHtml(CheckBoxMouseOverBackColorString);
		internal static readonly Color CheckBoxMouseDownBackColor = ColorTranslator.FromHtml(CheckBoxMouseDownBackColorString);
		internal static readonly Color CheckBoxSelectedBackColor = ColorTranslator.FromHtml(CheckBoxSelectedBackColorString);

		internal static readonly Color CheckBoxMouseOverBorderColor = ColorTranslator.FromHtml(CheckBoxMouseOverBorderColorString);
		internal static readonly Color CheckBoxMouseDownBorderColor = ColorTranslator.FromHtml(CheckBoxMouseDownBorderColorString);
		internal static readonly Color CheckBoxSelectedBorderColor = ColorTranslator.FromHtml(CheckBoxSelectedBorderColorString);

		internal static readonly Color CheckBoxMouseOverForeColor = ColorTranslator.FromHtml(CheckBoxMouseOverForeColorString);
		internal static readonly Color CheckBoxMouseDownForeColor = ColorTranslator.FromHtml(CheckBoxMouseDownForeColorString);
		internal static readonly Color CheckBoxSelectedForeColor = ColorTranslator.FromHtml(CheckBoxSelectedForeColorString);

		internal const string CheckBoxTickColorString = "1,79,114";
		internal const string CheckBoxIndeterminateBackColorString = "24,123,166"; //24,123,166
		internal const string CheckBoxFocusedBorderColorString = "1,79,114";

		internal const string CheckBoxMouseOverBackColorString = "223,223,223";
		internal const string CheckBoxMouseDownBackColorString = "White";
		internal const string CheckBoxSelectedBackColorString = "White";

		internal const string CheckBoxMouseOverBorderColorString = "223,223,223";
		internal const string CheckBoxMouseDownBorderColorString = "1,79,114";
		internal const string CheckBoxSelectedBorderColorString = "1,79,114";

		internal const string CheckBoxMouseOverForeColorString = "Black";
		internal const string CheckBoxMouseDownForeColorString = "Black";
		internal const string CheckBoxSelectedForeColorString = "Black";
		#endregion

		#region CombinedCheckBox
		internal static readonly Color CombinedCheckBoxSelectedColor = ColorTranslator.FromHtml(CombinedCheckBoxSelectedColorString);
		internal static readonly Color CombinedCheckBoxBackColor = ColorTranslator.FromHtml(CombinedCheckBoxBackColorString);

		internal const string CombinedCheckBoxSelectedColorString = "White";
		internal const string CombinedCheckBoxBackColorString = "1,79,114";
		#endregion

		#region RadioButton
		internal static readonly Color RadioButtonCentreForeColor = ColorTranslator.FromHtml(RadioButtonCentreForeColorString);
		internal static readonly Color RadioButtonFocusedBorderColor = ColorTranslator.FromHtml(RadioButtonFocusedBorderColorString);

		internal const string RadioButtonCentreForeColorString = "1,79,114";
		internal const string RadioButtonFocusedBorderColorString = "1,79,114";
		#endregion

		#region ComboBox
		internal static readonly Color ComboBoxButtonForeColor = ColorTranslator.FromHtml(ComboBoxButtonForeColorString);
		internal static readonly Color ComboBoxButtonBackColor = ColorTranslator.FromHtml(ComboBoxButtonBackColorString);

		internal static readonly Color ComboBoxMouseOverBorderColor = ColorTranslator.FromHtml(ComboBoxMouseOverBorderColorString);
		internal static readonly Color ComboBoxMouseDownBorderColor = ColorTranslator.FromHtml(ComboBoxMouseDownBorderColorString);

		internal const string ComboBoxButtonForeColorString = "1,79,114";
		internal const string ComboBoxButtonBackColorString = "White";

		internal const string ComboBoxMouseOverBorderColorString = "1,79,114";
		internal const string ComboBoxMouseDownBorderColorString = "1,79,114";
		#endregion

		#region DataGridView
		internal static readonly Color DataGridViewHeaderBackColor = ColorTranslator.FromHtml(DataGridViewHeaderBackColorString);
		internal static readonly Color DataGridViewHeaderForeColor = ColorTranslator.FromHtml(DataGridViewHeaderForeColorString);
		internal static readonly Color DataGridViewAlternationBackColor = ColorTranslator.FromHtml(DataGridViewAlternationBackColorString);

		internal const string DataGridViewHeaderBackColorString = "White";
		internal const string DataGridViewHeaderForeColorString = "Black";
		internal const string DataGridViewAlternationBackColorString = "White";
		#endregion

		#region DateTimePicker
		internal static readonly Color DateTimePickerMouseOverBorderColor = ColorTranslator.FromHtml(DateTimePickerMouseOverBorderColorString);
		internal static readonly Color DateTimePickerMouseDownBorderColor = ColorTranslator.FromHtml(DateTimePickerMouseDownBorderColorString);
		internal static readonly Color DateTimePickerSelectColor = ColorTranslator.FromHtml(DateTimePickerSelectColorString);

		internal const string DateTimePickerMouseOverBorderColorString = "1,79,114";
		internal const string DateTimePickerMouseDownBorderColorString = "1,79,114";
		internal const string DateTimePickerSelectColorString = "1,79,114";
		#endregion

		#region GroupList
		internal static readonly Color GroupBackColor = ColorTranslator.FromHtml(GroupBackColorString);
		internal static readonly Color GroupBorderColor = ColorTranslator.FromHtml(GroupBorderColorString);

		internal const string GroupBackColorString = "1,79,114";
		internal const string GroupBorderColorString = "1,79,114";
		#endregion

		#region Line
		internal static readonly Color LineBorderColor = ColorTranslator.FromHtml(LineBorderColorString);
		internal const string LineBorderColorString = "200,200,200";
		#endregion

		#region Tile
		internal static readonly Color PanoramaBackColor = ColorTranslator.FromHtml(PanoramaBackColorString);
		internal static readonly Color TileGroupForeColor = ColorTranslator.FromHtml(TileGroupForeColorString);

		internal static readonly Color TileBackColor = ColorTranslator.FromHtml(TileBackColorString);
		internal static readonly Color TileMouseOverBackColor = ColorTranslator.FromHtml(TileMouseOverBackColorString);
		internal static readonly Color TileMouseDownBackColor = ColorTranslator.FromHtml(TileMouseDownBackColorString);
		internal static readonly Color TileMouseOverBorderColor = ColorTranslator.FromHtml(TileMouseOverBorderColorString);

		internal static readonly Color TileForeColor = ColorTranslator.FromHtml(TileForeColorString);
		internal static readonly Color TileBordeColor = ColorTranslator.FromHtml(TileBorderColorString);

		internal const string PanoramaBackColorString = "White";
		internal const string TileGroupForeColorString = "Black";

		internal const string TileBackColorString = "252,252,252";
		internal const string TileMouseOverBackColorString = "White";
		internal const string TileMouseDownBackColorString = "247,247,247";
		internal const string TileMouseOverBorderColorString = "192,200,207";

		internal const string TileForeColorString = "Black";
		internal const string TileBorderColorString = "242,242,242";
		#endregion

		#region TabControl
		internal static readonly Color TabSelectedBottomBorderColor = ColorTranslator.FromHtml(TabSelectedBottomBorderColorString);

		internal static readonly Color TabBackColor = ColorTranslator.FromHtml(TabBackColorString);
		internal static readonly Color TabBorderColor = ColorTranslator.FromHtml(TabBorderColorString);
		internal static readonly Color TabForeColor = ColorTranslator.FromHtml(TabForeColorString);

		internal static readonly Color TabSelectedBackColor = ColorTranslator.FromHtml(TabSelectedBackColorString);
		internal static readonly Color TabSelectedBorderColor = ColorTranslator.FromHtml(TabSelectedBorderColorString);
		internal static readonly Color TabSelectedForeColor = ColorTranslator.FromHtml(TabSelectedForeColorString);

		internal static readonly Color TabMouseOverBackColor = ColorTranslator.FromHtml(TabMouseOverBackColorString);

		internal const string TabMouseOverBackColorString = "24,123,166";//177,233,217

		internal const string TabSelectedBottomBorderColorString = "24,123,166";
		internal const string TabBackColorString = "224,224,224";
		internal const string TabBorderColorString = "Transparent";

		internal const string TabForeColorString = "61,61,61";
		internal const string TabSelectedBackColorString = "24,123,166";
		internal const string TabSelectedBorderColorString = "Transparent";

		internal const string TabSelectedForeColorString = "White";//0,130,100

		#region 备份老的颜色
		/*internal const string TabSelectedBottomBorderColorString = "100,215,181";
        internal const string TabBackColorString = "0,127,102";
        internal const string TabBorderColorString = "Transparent";
        internal const string TabForeColorString = "White";
        internal const string TabSelectedBackColorString = "100,215,181";
        internal const string TabSelectedBorderColorString = "Transparent";
        internal const string TabSelectedForeColorString = "0,126,102";//1,79,114
        */
		#endregion

		#region TabControlCloseButton
		internal static readonly Color TabCloseButtonBackColor = ColorTranslator.FromHtml(TabCloseButtonBackColorString);
		internal static readonly Color TabCloseButtonBorderColor = ColorTranslator.FromHtml(TabCloseButtonBorderColorString);
		internal static readonly Color TabCloseButtonForeColor = ColorTranslator.FromHtml(TabCloseButtonForeColorString);
		internal static readonly Color TabCloseButtonMouseOverBackColor = ColorTranslator.FromHtml(TabCloseButtonMouseOverBackColorString);
		internal static readonly Color TabCloseButtonMouseOverBorderColor = ColorTranslator.FromHtml(TabCloseButtonMouseOverBorderColorString);
		internal static readonly Color TabCloseButtonMouseOverForeColor = ColorTranslator.FromHtml(TabCloseButtonMouseOverForeColorString);

		internal const string TabCloseButtonBackColorString = "Transparent";
		internal const string TabCloseButtonBorderColorString = "Transparent";
		internal const string TabCloseButtonForeColorString = "104,104,104";
		internal const string TabCloseButtonMouseOverBackColorString = "Red";
		internal const string TabCloseButtonMouseOverBorderColorString = "Transparent";
		internal const string TabCloseButtonMouseOverForeColorString = "White";
		#endregion
		#endregion

		#region SparkTitleBar
		internal static readonly Color TitleBackColor = ColorTranslator.FromHtml(TitleBackColorString);
		internal static readonly Color TitleBorderColor = ColorTranslator.FromHtml(TitleBorderColorString);
		internal static readonly Color TitleForeColor = ColorTranslator.FromHtml(TitleForeColorString);
		internal static readonly Color TitleMouseOverBackColor = ColorTranslator.FromHtml(TitleMouseOverBackColorString);
		internal static readonly Color TitleMouseDownBackColor = ColorTranslator.FromHtml(TitleMouseDownBackColorString);

		internal const string TitleBackColorString = "1,79,114";
		internal const string TitleBorderColorString = "Transparent";
		internal const string TitleForeColorString = "255,255,255";
		internal const string TitleMouseOverBackColorString = "133,221,195";
		internal const string TitleMouseDownBackColorString = "133,221,195";
		#endregion

		#region ToolStrip
		internal static readonly Color ToolStripBackColor1 = ColorTranslator.FromHtml(ToolStripBackColor1String);
		internal static readonly Color ToolStripBackColor2 = ColorTranslator.FromHtml(ToolStripBackColor2String);
		internal static readonly Color ToolStripImageMarginBackColor = ColorTranslator.FromHtml(ToolStripImageMarginBackColorString);
		internal static readonly Color ToolStripBackColor = ColorTranslator.FromHtml(ToolStripBackColorString);
		internal static readonly Color ToolStripMouseOverBackColor = ColorTranslator.FromHtml(ToolStripMouseOverBackColorString);
		internal static readonly Color ToolStripMouseDownBackColor = ColorTranslator.FromHtml(ToolStripMouseDownBackColorString);
		internal static readonly Color ToolStripSelectedBackColor = ColorTranslator.FromHtml(ToolStripSelectedBackColorString);
		internal static readonly GradientColor ToolStripHighlightColor = new GradientColor(
			MouseOverBackColor,
			SelectedBackColor,
			new float[] { 0.0F, 0.7F, 1.5F },
			new float[] { 0.0F, 0.6F, 1F }
		);
		internal const string ToolStripBackColor1String = "237,237,237";
		internal const string ToolStripBackColor2String = "237,237,237";
		internal const string ToolStripImageMarginBackColorString = "210,210,210";
		internal const string ToolStripBackColorString = "White";
		internal const string ToolStripMouseOverBackColorString = "1,79,114";
		internal const string ToolStripMouseDownBackColorString = "1,79,114";
		internal const string ToolStripSelectedBackColorString = "1,79,114";
		#endregion

		#region TreeView 
		internal static readonly Color TreeViewNodeSplitLineColor = ColorTranslator.FromHtml(TreeViewNodeSplitLineColorString);
		internal const string TreeViewNodeSplitLineColorString = "1,79,114";
		#endregion

		#region Splitter
		internal static readonly Color SplitterMouseOverBackColor = ColorTranslator.FromHtml(SplitterMouseOverBackColorString);
		internal const string SplitterMouseOverBackColorString = "1,79,114";
		#endregion

		#region NavigationBar
		internal static readonly Color NavigationBarTitleFontColor = ColorTranslator.FromHtml(NavigationBarTitleFontColorString);
		internal static readonly Color NavigationBarBackColor = ColorTranslator.FromHtml(NavigationBarBackColorString);
		internal static readonly Color NavigationBarToolBackColor = ColorTranslator.FromHtml(NavigationBarToolBackColorString);
		internal static readonly Color NavigationBarToolSelectedColor = ColorTranslator.FromHtml(NavigationBarToolSelectedColorString);

		internal const string NavigationBarTitleFontColorString = "0,130,100";
		internal const string NavigationBarBackColorString = "255,255,255";
		internal const string NavigationBarToolBackColorString = "223,223,223";
		internal const string NavigationBarToolSelectedColorString = "180,179,179";
		#endregion
	}
}