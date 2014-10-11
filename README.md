##Generating a polygon collider for a sprite at runtime

<img src="/Screenshots/Fence1.PNG"/>

Starting with the image we convert the texture to a binary image.

<img src="/Screenshots/FenceBinaryimage.PNG" width="150"/>

We perform some image processing techniques to get the outline of the image.
<div style="text-align: center;">
<img src="/Screenshots/FenceDilation.PNG" width="150" style="display: inline-block"/>
<img src="/Screenshots/FenceErosion.PNG" width="150" style="display: inline-block"/>
<img src="/Screenshots/FenceSubtraction.PNG" width="150" style="display: inline-block"/>
</div>

From this we follow the outline assigning each pixel as a vertex. We can allow islands( shown in a different colour) and concave shapes.

<img src="/Screenshots/FenceVerts.PNG" width="150"/>

Removing the unnecessary vertices we can simplify the collider greatly.

<img src="/Screenshots/FenceVertReduced.PNG" width="150"/>

This lets us generate a collider for complex sprites.

<img src="/Screenshots/FenceFinal.PNG" width="150"/>
