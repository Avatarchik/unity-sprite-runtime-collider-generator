##Generating a polygon collider for a sprite at runtime

![](/https://www.dropbox.com/s/e7vnx4qguv7kwxh/Fence1.PNG?dl=0)

Starting with the image we convert the texture to a binary image.
![](/https://www.dropbox.com/s/omcuqzx928h1mtd/FenceBinaryimage.PNG?dl=0)

We perform some image processing techniques to get the outline of the image.
![](/https://www.dropbox.com/s/7tu3jquamt4fbbv/FenceDilation.PNG?dl=0)
![](/https://www.dropbox.com/s/3yfydwwcieajsrn/FenceErosion.PNG?dl=0)
![](/https://www.dropbox.com/s/stx9zt6lfiwg1pq/FenceSubtraction.PNG?dl=0)

From this we follow the outline assigning each pixel as a vertex. We can allow islands( shown in a different colour) and concave shapes.
![](/https://www.dropbox.com/s/ykwdg59gbcr2h7u/FenceVerts.PNG?dl=0)

Removing the unnecessary vertices we can simplify the collider greatly.
![](/https://www.dropbox.com/s/f7f0oaqr17ct8ot/FenceVertReduced.PNG?dl=0)

This lets us generate a collider for complex sprites.
![](/https://www.dropbox.com/s/yp6sbumcosyca6k/FenceFinal.PNG?dl=0)
