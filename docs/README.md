# Produce documentation with DocFX


You can use `docfx init` to generate the `docfx.json` in the directory you want to build the documentation of the project.

## Adapt the `docfx.json` file to your nedd. 

In my case I have set relative source path (`src`) to ../SharpMiner/ where is the source code of library.

I have also modified the globalMetadata to include some information such as the name of the project, icon, favicon for the generated website.

```json
{
  "metadata": [
    {
      "src": [
        {
          "src": "../SharpMiner/",
          "files": [
            "**/*.csproj"
          ]
        }
      ],
      "dest": "api"
    }
  ],
  "build": {
    "content": [
      {
        "files": [
          "**/*.{md,yml}"
        ],
        "exclude": [
          "_site/**"
        ]
      }
    ],
    "resource": [
      {
        "files": [
          "images/**"
        ]
      }
    ],
    "output": "_site",
    "template": [
      "default",
      "modern"
    ],
    "globalMetadata": {
      "_appName": "SharpMiner",
      "_appTitle": "",
	  "_appFooter": "SharpMiner",
	  "_appLogoPath": "images/logo.png",
	  "_appFaviconPath": "images/favicon.png",
      "_enableSearch": true,
      "pdf": true
    }
  }
}
```

## Generate the library API documentation

using the following command I can generate the API documentation of the library. 

```
docfx metadata
```

This will use the XML documentation contained in the assembly to generate information about classes, methods, enums ...

## Build and serve the site 

```
docfx build docfx.json --serve
```

## Generate PDF document 

```
docfx pdf
```