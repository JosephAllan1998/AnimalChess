   M S B A M L   `   `   `  ����F  BAnimalChess, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:AnimalChessAnimalChess  5   	       AnimalChess.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   b ^MaterialDesignThemes.Wpf, Version=3.2.0.1979, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   %localclr-namespace:AnimalChess  5   	   -   5        $��thisMain��5   B     � Title$  	Cờ thú��5   	   $	��620��6   $	��500��6%    � WindowStartupLocation$ CenterScreen=�61    � WindowStyle$
 None=�5	   	    � WindowState$ Normal=�6    � 
ResizeMode$ NoResize=�5
   <   .��5      � ��5   
   �� 5       �� ImageSource$&  /Resources/Images/background.jpg��6   -   5        
bgGrid�� j� TileMode$
 Tile=�5       j� Stretch$
 Fill=�6)    �� Opacity$	 0.3Q�68   	 � RowDefinitions	 5   
   �� 5      $
��auto �6   �� 5      $��* �6   .+�5   
   ]� $��0��6   ��6!   �� 
 �� ElementName$
 thisMain�� �� Path$ ActualWidth�.+�5      o� -   5        $��dropDownMenu��5   b    o� 
IsMainMenu . $��20��6%   $
��Left=�61   $y�Visible=�6M   .@�5      m� $
��Game��6   $��20��6)      MenuItemHeader#��[  65   $��3 0 0 0q�6]   .@�5      m� -   5        $��New game��5       m� InputGestureText$ Ctrl+N��61     MenuItemSub#��[ 6K    m� Icon 5      0  *MaterialDesignThemes.Wpf.PackIconExtension     Kind$ NewBox=�6L   m� 5      $
��Mode��6   #��[ 6-    5        $ ApplicationSettings=�.@�5      m� #��[ 6#   ��5       �� 5!   "   .R�5"   &   �� .+�5#   *   	� -   5         	� 	GroupName$
 Mode��5#   6   $��   One Player��6G   $��0,0,0,5q�6_    d� 	IsChecked$
 TrueY�5$   6   6G   	� 5%   *   -   5        $
 Mode��5%   6   $��   Two Player��6G   $ FalseY�5&   6   6H   5'   '   5)   #   5+      5,      m� 5.      -   5        $��miLevel��5/   U   $��Level��5.      #��[ 6.   {�.  5/        $ CarCoolantLevel=�.@�50      m� #��[ 6#   ��51      �� 52   "   .R�53   &   �� .+�54   *   	� -   5        $ Level��54   6   $��   Easy��6H   $
 TrueY�55   6   6G   	� 56   *   -	   5        $ Level��56   6   $��	   Medium��6H   $	��0,5q�6\   $ FalseY�57   6   6H   	� 58   *   -
   5        $ Level��58   6   $��   Hard��6H   $ FalseY�59   6   6H   5:   '   5<   #   5>      5?      �� 5A      
���   �6    m� 5C      -   5        $��miUndo��5E   1   $
��Undo��5C      $ Ctrl+Z��6-   #��[ 6G   {�.   5D        $
 Undo=�5E   1   m� 5G      -   5        $��miRedo��5I   1   $
��Redo��5G      $ Ctrl+Y��6-   #��[ 6G   {�.   5H        $
 Redo=�5I   1   m� 5K      -   5        $��miResetGame��5L      $��Restart��5K      $ Ctrl+R��60   #��[ 6J    � ToolTip$ 
Reset game��5L   ^   {�.   62     $ Power=�5M   7   �� 5O      
���   �6    m� 5Q      -   5        $��
miSaveGame��5R      $
��Save��5Q      $ Ctrl+S��6-   #��[ 6G   {�.   5S        $ ContentSave=�6Q   m� 5U      -   5        $��Reload��5U      $ Ctrl+L��6/   #��[ 6I   $ Reload last saved��5V   L    6     $ Reload=�6h   �� 5X      
���   �6    m� 5Z      -   5        $��	Quit game��5Z      $ Alt+F4��62   #��[ 6L    5[        $ 	ExitToApp=�6O   5\      m� 5^      -   5        $��Help?��5^      $��20��6*   #��[  66   6^   5_      �� 5a       �� Orientation$ 
Horizontal=�6   $��Right=�62   $��0 0 0 0q�6N   .+�5b      �� -   5        $-��'/Resources/Images/Controls/collapse.png��5b        imgWindowsStyle#��[ 5c      $ Collapse Menu��6A   �� 5e      -   5        $)��#/Resources/Images/Controls/gear.png��5e      $ Setting��6E   $��	10,0,20,0q�5f   D   #��[ 5g      �� 5i      -   5        $-��'/Resources/Images/Controls/minimize.png��5i      #��[ 5j      6A   �� 5l      -   5        $-��'/Resources/Images/Controls/maximize.png��5l      #��[ 6I   5m      �� 5o      -   5        $*��$/Resources/Images/Controls/close.png��5o      #��[ 5p      5q      5r      � 5t   
   $��1��6   .+�5u       � -   5        $��	frameLoad��5u        � NavigationUIVisibility$ Hidden=�6%   5v      5w      5x      