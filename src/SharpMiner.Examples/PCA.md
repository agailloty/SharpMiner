# Steps to compute PCA

* Compute eigenvectors and eignevalues
* Compute Principal Compnents from the eigenvalues
* Project the data

There are three ways to computes the eigenvalues : 
* With eignevalues
* Singular value decomposition
* NIPALS

After we compute the eignevalues and eignevectors we can compute the principal components from the eigenvectors


# Eigenvectors

They are computed on the values returned by the singular values decomposition (SVD) : they are equal to the v transposed