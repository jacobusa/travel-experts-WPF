   M S B A M L   `   `   `  ����G  CWPFApp_Cloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WPFApp_CloudWPFApp_Cloud  5   	       WPFApp_Cloud.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:WPFApp_Cloud     5   	   V RMaterialDesignThemes.Wpf, Version=3.2.0.1479, Culture=neutral, PublicKeyToken=nullZ VMaterialDesignThemes.MahApps, Version=0.1.2.1479, Culture=neutral, PublicKeyToken=nullHmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes   5   	     � Title$  Agents Portal��5	   	   $	��450��6   $	��800��6,   .��5
      � -   5        ���	#FFEEEEEE.+�5   
   �� $	��Top=�6   .+�5      � ���	#FF0069C0$��10��6*   5      � 5   
   $��	250 0 0 0q�6   .+�5      � -   5         �Name$ GridPrincipal��5      $��Center=�6*   $��Center=�6G   5      � 5   
   $	��250��6   $
��Left=�6   ���	#FF222222.+�5      ?  9MaterialDesignThemes.Wpf.Transitions.TransitioningContent  -   5          Name$ TrainsitionigContentSlide��5   2   C  =MaterialDesignThemes.Wpf.Transitions.TransitioningContentBase   OpeningEffect 6U   D  >MaterialDesignThemes.Wpf.Transitions.TransitionEffectExtension  *SlideInFromLeft+   Duration 0:0:0.2.��5      � -   5        $ 
GridCursor��5      $��	0 100 0 0q�6+   ���	#FF0069C0$��10��6U   $
��Left=�6`   $��60��6{   $	��Top=�6�   5      �� 5      -   5         ��Name$ ListViewMenu��5      $��
0 100 0 70q�6Y   
�������$��Champagne & Limousines)�6�   $��18(�6�    �� SelectedIndex$ 0��6�   {�6-   ��  �� Path$ IsAuthenticated�.@�5      �� $��60��6   .��5      ��  �� Orientation$ 
Horizontal=�6!   .+�5      '  !MaterialDesignThemes.Wpf.PackIcon  	   Kind$
	 Home=�62   $��30��6>   $��30��6I   $��Center=�6U   $��5q�6p   �� 5      $��Login��6$   $��17(�61   $��Center=�6?   $
��20 0q�6Z   5      5      �� 5      $��60��6   .��5      �� $ 
Horizontal=�6!   .+�5         $	 Globe=�62   $��30��6?   $��30��6J   $��Center=�6V   $��5q�6q   �� 5!      $��Packages List��6$   $��17(�69   $��Center=�6G   $
��20 0q�6b   5"      5#      �� 5$      $��60��6   .��5%      �� $ 
Horizontal=�6!   .+�5&        $
	 Plus=�62   $��30��6>   $��30��6I   $��Center=�6U   $��5q�6p   �� 5'      $��Add New Package��6$   $��17(�6;   $��Center=�6I   $
��20 0q�6d   5(      5)      �� 5*      $��60��6   .��5+      �� $ 
Horizontal=�6!   .+�5,        $	 Create=�62   $��30��6@   $��30��6K   $��Center=�6W   $��5q�6r   �� 5-      $��Edit Package��6$   $��17(�68   $��Center=�6F   $
��20 0q�6a   5.      5/      �� 50      $��60��6   .��51      �� $ 
Horizontal=�6!   .+�52        $	 Layers=�62   $��30��6@   $��30��6K   $��Center=�6W   $��5q�6r   �� 53      $��Products List��6$   $��17(�69   $��Center=�6G   $
��20 0q�6b   54      55      �� 56      $��60��6   .��57      �� $ 
Horizontal=�6!   .+�58        $	 AddToPhotos=�62   $��30��6E   $��30��6P   $��Center=�6\   $��5q�6w   �� 59      $��Add Product��6$   $��17(�67   $��Center=�6E   $
��20 0q�6`   5:      5;      �� 5<      $��60��6   .��5=      �� $ 
Horizontal=�6!   .+�5>        $	 LayersSearch=�62   $��30��6F   $��30��6Q   $��Center=�6]   $��5q�6x   �� 5?      $��Edit Product��6$   $��17(�68   $��Center=�6F   $
��20 0q�6a   5@      5A      �� 5B      $��60��6   .��5C      �� $ 
Horizontal=�6!   .+�5D        $	 People=�62   $��30��6@   $��30��6K   $��Center=�6W   $��5q�6r   �� 5E      $��Suppliers List��6$   $��17(�6:   $��Center=�6H   $
��20 0q�6c   5F      5G      �� 5H      $��60��6   .��5I      �� $ 
Horizontal=�6!   .+�5J        $	 	PeopleAdd=�62   $��30��6C   $��30��6N   $��Center=�6Z   $��5q�6u   �� 5K      $��Add Supplier��6$   $��17(�68   $��Center=�6F   $
��20 0q�6a   5L      5M      �� 5N      $��60��6   .��5O      �� $ 
Horizontal=�6!   .+�5P        $	 PersonOutline=�62   $��30��6G   $��30��6R   $��Center=�6^   $��5q�6y   �� 5Q      $��Edit Supplier��6$   $��17(�69   $��Center=�6G   $
��20 0q�6b   5R      5S      5T      �� 5U      $��Center=�6   $��0,21,0,0q�61   $��30(�6C   
�������$��Agents Portal��6d   $	��Top=�6|   $	��200��6�   �� 5V      $��Bottom=�6   .+�5W      �� -   5         "  MaterialDesignRaisedDarkButton#���   5W      $��Bottom=�6R   $��Logout��6m   
���  ��$��17(�6�   $��44��6�   6�   �� 5X      -   5        #���   5X      $��Bottom=�6R   $
��Quit��6m   $��17(�6�   $��44��6�   6�   5Y      5Z      5\      5]      