##Generating a polygon collider for a sprite at runtime

<img src="/Screenshots/Fence1.PNG"/>

Starting with the image we convert the texture to a binary image.
<img src="/Screenshots/FenceBinaryimage.PNG" style="width: 100px;"/>

We perform some image processing techniques to get the outline of the image.
<div style="position:center">
<img src="/Screenshots/FenceDilation.PNG" style="width: 100px; display: inline-block"/>
<img src="/Screenshots/FenceErosion.PNG" style="width: 100px; display: inline-block"/>
<img src="/Screenshots/FenceSubtraction.PNG" style="width: 100px; display: inline-block"/>
</div>

From this we follow the outline assigning each pixel as a vertex. We can allow islands( shown in a different colour) and concave shapes.
<img src="/Screenshots/FenceVerts.PNG" style="width: 100px;"/>

Removing the unnecessary vertices we can simplify the collider greatly.
<img src="/Screenshots/FenceVertReduced.PNG" style="width: 100px;"/>

This lets us generate a collider for complex sprites.
<img src="/Screenshots/FenceFinal.PNG" style="width: 100px;"/>
