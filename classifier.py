import numpy as np
import ifcopenshell
import ifcopenshell.api.pset


def classify_shape(dimensions, i_axis_z):
    x_initial, y_initial, z_initial = dimensions
    dimensions_rotated = [0, 0, 0]
    dimensions_rotated[i_axis_z] = z_initial
    dimensions_rotated[(i_axis_z + 1) % 3] = x_initial
    dimensions_rotated[(i_axis_z + 2) % 3] = y_initial
    x, y, z = dimensions_rotated

    if x > 2000 and x < 15000 and y > 150 and y < 1000 and z > 150 and z < 1000:
        return "beam"
    elif y > 2000 and y < 15000 and x > 150 and x < 1000 and z > 150 and z < 1000:
        return "beam"
    elif x > 200 and x < 2000 and y > 200 and y < 2000 and z > 2000 and z < 15000:
        return "column"
    elif x > 2000 and x < 15000 and y > 100 and y < 500 and z > 2000 and z < 15000:
        return "wall"
    elif y > 2000 and y < 15000 and x > 100 and x < 500 and z > 2000 and z < 15000:
        return "wall"
    elif x < 200 and y < 200 and z < 200:
        return "small"
    else:
        return "other"


def classify(file_name):
    with open(file_name, "r") as file:
        nodes = []
        for line in file.readlines():
            if len(line) == 0 or line[0] != "#":
                continue
            if len(node := line.split("=", maxsplit=1)) < 2:
                continue
            else:
                id_node, content_node = node

                type_node, parameters_node_unparsed = content_node.strip().split(
                    "(", maxsplit=1
                )
                nodes.append(
                    (id_node.strip(), type_node, parameters_node_unparsed[:-2])
                )

        shapes = []
        directions = []

        # TODO Safer parsing
        def parse_references(parameters_node):
            ids_references = []
            for parameter in parameters_node.split(","):
                if len(parameter) == 0:
                    continue

                if parameter[0] == "(":
                    parameter = parameter[1:]

                if parameter[0] == "#":
                    if parameter[-1] == ")":
                        parameter = parameter[:-1]
                    ids_references.append(parameter)
            return ids_references

        def get_referenced_coordinates(id_node, type_node, parameters_node):
            if type_node == "IFCCARTESIANPOINT":
                return [tuple(map(float, parameters_node[1:-1].split(",")))]
            else:

                ids_references = parse_references(parameters_node)

                result = []
                for id_reference in ids_references:
                    node_reference = next(
                        node_reference
                        for node_reference in nodes
                        if node_reference[0] == id_reference
                    )
                    result += get_referenced_coordinates(*node_reference)
                return result

        for id_node, type_node, parameters_node in nodes:
            if type_node == "IFCPRODUCTDEFINITIONSHAPE":
                id_reference = parse_references(parameters_node)[-1]
                node_reference = next(
                    node_reference
                    for node_reference in nodes
                    if node_reference[0] == id_reference
                )
                coordinates_referenced = get_referenced_coordinates(*node_reference)
                if (
                    len(
                        list(
                            filter(
                                lambda coordinates_point: len(coordinates_point) != 3,
                                coordinates_referenced,
                            )
                        )
                    )
                    > 0
                ):
                    continue

                coordinates_referenced = np.array(coordinates_referenced)
                coordinates_min = np.min(coordinates_referenced, axis=0)
                coordinates_max = np.max(coordinates_referenced, axis=0)
                dimensions = coordinates_max - coordinates_min
                shapes.append(
                    (
                        id_node,
                        dimensions,
                    )
                )
            elif type_node == "IFCLOCALPLACEMENT":
                id_reference = parse_references(parameters_node)[-1]
                node_reference = next(
                    node_reference
                    for node_reference in nodes
                    if node_reference[0] == id_reference
                )
                (
                    id_node_coordinate_directions,
                    type_node_coordinate_directions,
                    parameters_node_coordinate_directions,
                ) = node_reference
                if type_node_coordinate_directions != "IFCAXIS2PLACEMENT3D":
                    continue
                else:
                    id_reference_direction_z = parse_references(
                        parameters_node_coordinate_directions
                    )[1]
                    node_direction_z = next(
                        node_reference
                        for node_reference in nodes
                        if node_reference[0] == id_reference_direction_z
                    )

                    if node_direction_z[1] != "IFCDIRECTION":
                        continue
                    else:
                        direction_z = tuple(
                            map(float, node_direction_z[2][1:-1].split(","))
                        )
                        for i_axis in range(3):
                            if direction_z[i_axis] != 0:
                                directions.append((id_node, i_axis))
                                break

        model = ifcopenshell.open("E:/code/junction2024/data/DummyModel.ifc")

        def get_classification(node):
            ids_references_node = parse_references(node[2])

            found_shape = False
            for id_shape, dimensions_shape in shapes:
                if id_shape in ids_references_node:
                    found_shape = True
                    break
            found_direction = False
            for id_direction, i_direction_z in directions:
                if id_direction in ids_references_node:
                    found_direction = True
                    break

            if found_direction and found_shape:
                return (node[0], classify_shape(dimensions_shape, i_direction_z))
            else:
                return None

        nodes_classified = filter(
            lambda classification: classification is not None,
            map(get_classification, nodes),
        )

        for id_node, class_node in nodes_classified:
            product = model.by_id(int(id_node[1:]))
            pset = ifcopenshell.api.pset.add_pset(
                model, product, "beaminator_classification"
            )

            ifcopenshell.api.pset.edit_pset(
                model, pset=pset, properties={"class": class_node}
            )
    model.write("DummyModel_classified.ifc")


if __name__ == "__main__":
    classify("DummyModel.ifc")
