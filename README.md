##Generating a polygon collider for a sprite at runtime

![](/Screenshots/Fence1.png)

Starting with the image we convert the texture to a binary image.
![](/Screenshots/FenceBinaryimage.PNG)

We perform some image processing techniques to get the outline of the image.
![](/Screenshots/FenceDilation.PNG)
![](/Screenshots/FenceErosion.PNG)
![](/Screenshots/FenceSubtraction.PNG)

From this we follow the outline assigning each pixel as a vertex. We can allow islands( shown in a different colour) and concave shapes.
![](/Screenshots/FenceVerts.PNG)

Removing the unnecessary vertices we can simplify the collider greatly.
![](/Screenshots/FenceVertReduced.PNG)

This lets us generate a collider for complex sprites.
![](/Screenshots/FenceFinal.PNG)
