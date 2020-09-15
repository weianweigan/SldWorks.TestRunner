# Documentation

## Intension

The goal of this add-in is to write test for your add-in. Because it is not possible to start revit as a 'service' or start from a test, it is necessary to start Revit first, then start your tests. In this case, the Revit context is available in the tests.

## Writing Tests

First add the NuGet package of [NUnit](https://www.nuget.org/packages/NUnit/) to the test project.

A test must be marked with the ```Test``` Attribute of the NUnit 3 library. All marked methods will be recognized when the test assembly is loaded. A ```Test``` is executable. 
A method marked with the ```SetUp``` Attribute will be executed before each test.
A method marked with the ```TearDown``` Attribute will be executed after each test.

```C#
[SetUp]
public void MySetUp(){
    // Do some stuff before the test runs.
}

[TearDown]
public void MyTearDown(){
    // Do some stuff after the test is finished
}

[Test]
public void MyTest(){
    // Do some test stuff
}
```


To get SolidWorks API objects like ```ISldWorks``` , extend the test method signature with one or both of the called Classes. The injected objects can be used to do some stuff, for example open a file.

```C#
[Test]
public void MyTest( UIApplication uiApplication){
    // Do some test stuff. ex.:
    uiApplication.Application.OpenDocumentFile( "C:\myTestFile.rvt" );
}
```

A sample test assembly is included in the visual studio solution.

## VisualStudio Solution

### Sample Test Project

Containing some sample Tests, showing how they could be implemented.

### Build the Solution

The CADFrameWorks developed by https://xcad.xarial.com/ is Used for this project.Build action will auto registed addin for solidworks. Clear action will delete the addin inforamtion. 

## Precompiled binaries

The compiled add-in is also available in the [install](../install) section. Download the whole directory and place it somewhere. Run the corresponding .cmd to install the add-in.