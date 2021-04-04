# Parallax.WPF
Parallax Effect for WPF

## This is a fork from https://github.com/Pyrathlon/Parallax.WPF where I added a Nuget package

# Install with Nuget

```Powershell
Install-Package Parallax.WPF
```

# After you installed Parallax.WPF
* Add reference : 
```XAML
xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
xmlns:parallax="clr-namespace:Parallax.WPF;assembly=Parallax.WPF"
```

* Use the effect like this :
```XAML
<Image Source="Images/Backgrounds/cosmo_bg.PNG" Margin="-30" Stretch="UniformToFill"
                     parallax:ParallaxEffect.IsBackground="True"
                     parallax:ParallaxEffect.Parent="{Binding ElementName=TopGrid}"
                     parallax:ParallaxEffect.UseParallax="True"
                     parallax:ParallaxEffect.XOffset="50"
                     parallax:ParallaxEffect.YOffset="50">
    <i:Interaction.Behaviors>
        <parallax:ParallaxEffect/>
    </i:Interaction.Behaviors>
</hc:GifImage>
```

## Demo 


https://user-images.githubusercontent.com/17864005/113509902-49a2d180-9558-11eb-9ebf-711a2ea33e2a.mp4


