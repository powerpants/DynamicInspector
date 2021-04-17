# DynamicInspector
A Simple Custom Unity Inspector Editor that allows you use Attribute to hide properties or make it readonly dynamically.

### Usage
+ Add class attribute CustomInspectorGUI to enable.
+ Add field attribute DynamicHidden to which field you want to hide in Inspector.
+ The DynamicHidden attribute needs three parameters:
    - the first one is a string indicate the reference field. 
    - the second one is a value indicate when to hide.
    - the third one is a bool option indicate whether to hide or non-writable, default is to hide. 

### Sample
* Here is a sample.  
![sample](https://raw.githubusercontent.com/powerpants/DynamicInspector/main/Doc/codev1.png "code")  
![sample](https://raw.githubusercontent.com/powerpants/DynamicInspector/main/Doc/Inspector1v1.png "code")  
![sample](https://raw.githubusercontent.com/powerpants/DynamicInspector/main/Doc/Inspector2v1.png "code")  
