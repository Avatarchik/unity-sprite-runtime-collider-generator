##Generating a polygon collider for a sprite at runtime

![](/Screenshots/Fence1.PNG =100x100)

Starting with the image we convert the texture to a binary image.
![](/Screenshots/FenceBinaryimage.PNG =100x100)

We perform some image processing techniques to get the outline of the image.
![](/Screenshots/FenceDilation.PNG =100x100)
![](/Screenshots/FenceErosion.PNG =100x100)
![](/Screenshots/FenceSubtraction.PNG =100x100)

From this we follow the outline assigning each pixel as a vertex. We can allow islands( shown in a different colour) and concave shapes.
![](/Screenshots/FenceVerts.PNG =100x100)

Removing the unnecessary vertices we can simplify the collider greatly.
![](/Screenshots/FenceVertReduced.PNG =100x100)

This lets us generate a collider for complex sprites.
![](/Screenshots/FenceFinal.PNG =100x100)
