   M S B A M L   `   `   `  ����A  =YT2MP3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:YT2MP3YT2MP3  5   	       YT2MP3.AlbumWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:YT2MP3     5   	     � Title$  AlbumWindow��5   	   $	��450��6   $	��800��6*   .��5	      �� .+�5
   
   �� -   5         ��Name$ 	artistTxt��5
      $	��200��6%   .��61   Artist69   �� 5   
   -   5        $ albumNameTxt��5      $	��200��6(   .��64   
Album Name6@   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ 
MyDataGrid��5         AutoGenerateColumns$ False��6'      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Title��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 64   �� *Title+  5      $ Start Of Track (mm:ss)��6%    6E   �� *	StartTime+5      �� 5   
   -   5         ��Name$ Submit��5      $
��0 20q�6!   $��Submit��6/   $	��200��6@   6c   �� 5   
   -   5         ��Name$ messageTxtAlbum��5      � 5   
   -   5        	 �Name$	 dlProgressAlbum��5   )   $��	0,30,0,30q�6   $��10��6C   $	��350��6O   
 � Maximum$
 1Q�6[   �� 5   
   -   5        $ dirTxtAlbum��5      $	��200��6(   .��64   $"D:\Users\Mark\Music\Youtube Albums6X   5      5      