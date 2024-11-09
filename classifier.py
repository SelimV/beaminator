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
                node_reference_prime = next(
                    node_reference
                    for node_reference in nodes
                    if node_reference[0] == id_reference
                )
                shapes.append(
                    (id_node, get_referenced_coordinates(*node_reference_prime))
                )

        for id_shape, coordinates_points_shape in shapes:
            print(f"{id_shape=}, {coordinates_points_shape=}")


if __name__ == "__main__":
    classify("DummyModel.ifc")
