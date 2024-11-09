import ifcopenshell

# Load the IFC model
model_path = "D:/Junction Events/IFC/IFC example models/DummyModel.ifc"  # Replace with your path
model = ifcopenshell.open(model_path)

# Define categories to extract and analyze for IFC2X3 schema
element_data = {
    "beams": model.by_type("IfcBeam"),
    "columns": model.by_type("IfcColumn"),
    "walls": model.by_type("IfcWall"),
    "slabs": model.by_type("IfcSlab"),
    "windows": model.by_type("IfcWindow"),
    "doors": model.by_type("IfcDoor")
}

# Extract relevant data for each element type
for element_type, elements in element_data.items():
    print(f"\n{element_type.capitalize()}:")
    for element in elements:
        element_info = {
            "Name": element.Name,
            "Description": element.Description,
            "ObjectType": element.ObjectType,
            "Material": None,
            "Dimensions": {
                "Width": None,
                "Depth": None,
                "Height": None,
                "Length": None  # Add length for elements like beams
            },
            "PhysicalProperties": {
                "Weight": None,
                "Volume": None,
                "GrossFootprintArea": None,
                "AreaPerTon": None,
                "NetSurfaceArea": None
            }
        }

        # Check if this is a floor slab
        if element_type == "slabs" and getattr(element, "PredefinedType", None) == "FLOOR":
            element_info["Type"] = "Floor Slab"

        # Attempt to get material data
        if element.HasAssociations:
            for association in element.HasAssociations:
                if association.is_a("IfcRelAssociatesMaterial"):
                    material = association.RelatingMaterial
                    element_info["Material"] = material.Name if material else None

        # Check for dimensions and physical properties in property sets (Psets)
        if element.IsDefinedBy:
            for definition in element.IsDefinedBy:
                if definition.is_a("IfcRelDefinesByProperties"):
                    property_set = definition.RelatingPropertyDefinition
                    if property_set.is_a("IfcPropertySet"):
                        for prop in property_set.HasProperties:
                            # Capture dimensions
                            if prop.Name == "Width":
                                element_info["Dimensions"]["Width"] = prop.NominalValue.wrappedValue
                            elif prop.Name == "Depth":
                                element_info["Dimensions"]["Depth"] = prop.NominalValue.wrappedValue
                            elif prop.Name in ["Height", "Thickness"]:
                                element_info["Dimensions"]["Height"] = prop.NominalValue.wrappedValue
                            elif prop.Name == "Length":
                                element_info["Dimensions"]["Length"] = prop.NominalValue.wrappedValue

                            # Capture physical properties
                            elif prop.Name == "Weight":
                                element_info["PhysicalProperties"]["Weight"] = prop.NominalValue.wrappedValue
                            elif prop.Name == "Volume":
                                element_info["PhysicalProperties"]["Volume"] = prop.NominalValue.wrappedValue
                            elif prop.Name == "Gross Footprint Area":
                                element_info["PhysicalProperties"][
                                    "GrossFootprintArea"] = prop.NominalValue.wrappedValue
                            elif prop.Name == "AreaPerTon":
                                element_info["PhysicalProperties"]["AreaPerTon"] = prop.NominalValue.wrappedValue
                            elif prop.Name == "NetSurfaceArea":
                                element_info["PhysicalProperties"]["NetSurfaceArea"] = prop.NominalValue.wrappedValue

        # Display the data
        print(element_info)
