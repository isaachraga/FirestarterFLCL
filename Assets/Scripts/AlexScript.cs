using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlexScript : MonoBehaviour
{
    /*
    from PIL import Image
    import sys

    maze_file = input("Maze file: ")

    maze = Image.open(maze_file)
    maze_length = maze.size[0]
    maze = maze.convert('RGB')
    print(f'Maze Length: {maze_length}')

    #The two loops below automate the enterence of the start and end nodes

    f_r = 0

    while f_r<maze_length:
    if maze.getpixel((f_r, 0)) == (255, 255, 255):
    start = [f_r, 0]

        f_r += 1

    l_r = 0

    while l_r<maze_length:
    if maze.getpixel((l_r, maze_length - 1)) == (255, 255, 255):
    end = [l_r, maze_length - 1]

        l_r += 1

    black = (0, 0, 0)
    white = (255, 255, 255)

    maze.show()

    ##################################################################################### Initial Dead Ends ######################################################################

    w = 1
    v = 1
    c_c = []

    while w <= maze_length - 2:
    while v <= maze_length - 2:
    center = maze.getpixel((v, w))

    if center == white:
    up = maze.getpixel((v, w - 1))
    down = maze.getpixel((v, w + 1))
    left = maze.getpixel((v - 1, w))
    right = maze.getpixel((v + 1, w))

    if (((up == white) + (down == white) + (left == white) + (right == white)) == 1):
    maze.putpixel((v, w), (0, 0, 0))

    if up == white:
    c_c.append((v, w - 1))
    if down == white:
    c_c.append((v, w + 1))
    if left == white:
    c_c.append((v - 1, w))
    if right == white:
    c_c.append((v + 1, w))
    v += 1
    w += 1
    v = 1

    print(len(c_c))

    #################################################################################### Resulting Dead Ends #####################################################################

    n = 1
    c = 0
    new_c_c = []

    while n != 0:
    n = 0

    while c<len(c_c):
    center = maze.getpixel(c_c[c])

    up = maze.getpixel((c_c[c][0], c_c[c][1] - 1))
    down = maze.getpixel((c_c[c][0], c_c[c][1] + 1))
    left = maze.getpixel((c_c[c][0] - 1, c_c[c][1]))
    right = maze.getpixel((c_c[c][0] + 1, c_c[c][1]))

    if (((up == white) + (down == white) + (left == white) + (right == white)) == 1) and(center == white) :
    maze.putpixel(c_c[c], (0, 0, 0))

    n += 1

    if up == white:
    new_c_c.append((c_c[c][0], c_c[c][1] - 1))
    if down == white:
    new_c_c.append((c_c[c][0], c_c[c][1] + 1))
    if left == white:
    new_c_c.append((c_c[c][0] - 1, c_c[c][1]))
    if right == white:
    new_c_c.append((c_c[c][0] + 1, c_c[c][1]))
    c += 1
    c = 0

    c_c.clear()
    c_c = new_c_c.copy()
    new_c_c.clear()

    print(n)

    maze.show()

    ##################################################################################### Section 1: Nodes #####################################################################

    nodes = { }

    nodes[f'node 0'] = [start[0], start[1]]

    r = [[] for i in range(maze_length)]
    c = [[] for i in range(maze_length)]

    r[start[1]].append(start)
    c[start[0]].append(start)

    a = 1
    b = 1
    number = 0

    while b<maze_length - 1:
    while a<maze_length - 1:
    center = maze.getpixel((a, b))

    up = maze.getpixel((a, b - 1))
    down = maze.getpixel((a, b + 1))
    left = maze.getpixel((a - 1, b))
    right = maze.getpixel((a + 1, b))

    up_to_down_walls = (up == black) and(down == black) and(right == white) and(left == white)
    left_to_right_walls = (right == black) and(left == black) and(up == white) and(down == white)

    if (center == white) and(not (left_to_right_walls or up_to_down_walls)):
    number += 1
    nodes[f'node {number}'] = [a, b]

        r[b].append([a, b])
    c[a].append([a, b])

    a += 1
    b += 1
    a = 1

    nodes[f'node {len(nodes)}'] = [end[0], end[1]]

    number += 1

    r[end[1]].append(end)
    c[end[0]].append(end)

    print(f'Number of Nodes: {len(nodes)}')

    ################################################################################ Section 2: Node Connections ###############################################################

    connected_nodes = {}
    pair = 1

    r_num_big = 1
    r_num_small = 0

    while r_num_big<maze_length:
    while r_num_small<len(r[r_num_big]) - 1:

    distance = r[r_num_big][r_num_small + 1][0] - r[r_num_big][r_num_small][0]

    dist = 1
    between = 0

    while dist<distance:
    if maze.getpixel((r[r_num_big][r_num_small][0] + dist, r[r_num_big][r_num_small][1])) == (0, 0, 0):
    between += 1

    dist += 1

    if between == 0:
    connected_nodes[f'pair {pair}'] = [r[r_num_big] [r_num_small], r[r_num_big][r_num_small + 1]]
    pair += 1

    r_num_small += 1

    r_num_big += 1
    print(r_num_big)
    r_num_small = 0

    c_num_big = 1
    c_num_small = 0

    while c_num_big<maze_length:
    while c_num_small<len(c[c_num_big]) - 1:
    distance = c[c_num_big][c_num_small + 1][1] - c[c_num_big][c_num_small][1]

    dist = 1
    between = 0

    while dist<distance:
    if maze.getpixel((c[c_num_big][c_num_small][0], c[c_num_big][c_num_small][1] + dist)) == (0, 0, 0):
    between += 1

    dist += 1

    if between == 0:
    connected_nodes[f'pair {pair}'] = [c[c_num_big] [c_num_small], c[c_num_big][c_num_small + 1]]
    pair += 1

    c_num_small += 1

    c_num_big += 1
    print(c_num_big + r_num_big)
    c_num_small = 0

    connected_nodes_distances = {}
    p = 1

    while p <= len(connected_nodes) :
    connected_nodes_distances[f"pair {p}"] = (connected_nodes[f"pair {p}"][1][0] - connected_nodes[f"pair {p}"][0][0]) + (connected_nodes[f"pair {p}"][1][1] - connected_nodes[f"pair {p}"][0][1])
    p += 1

    ######################################################################## Section 3: Implementing Dijkstra's Algorithm ######################################################

    key_list = list(nodes.keys())
    val_list = list(nodes.values())

    path = {}
    path_extra = {}
    path_old = {}

    p = 1

    path['node 0'] = [0, "none"]

    while f'node {len(nodes) - 1}' not in path.keys():
        node = list(path.keys())[0]
        while p <= len(connected_nodes) :
            if (nodes[node] == connected_nodes[f'pair {p}'][0]):
                if key_list[val_list.index(connected_nodes[f'pair {p}'][1])] not in path_old.keys():
                    if key_list[val_list.index(connected_nodes[f'pair {p}'][1])] in path.keys():
                        if connected_nodes_distances[f'pair {p}'] + path[node][0] < path[key_list[val_list.index(connected_nodes[f'pair {p}'][1])]][0]:
                            path_extra[key_list[val_list.index(connected_nodes[f'pair {p}'][1])]] = [connected_nodes_distances[f'pair {p}'] + path[node][0], node]
    else:
    path_extra[key_list[val_list.index(connected_nodes[f'pair {p}'][1])]] = [connected_nodes_distances[f'pair {p}'] + path[node][0], node]
    if (nodes[node] == connected_nodes[f'pair {p}'][1]):
    if key_list[val_list.index(connected_nodes[f'pair {p}'][0])] not in path_old.keys():
    if key_list[val_list.index(connected_nodes[f'pair {p}'][0])] in path.keys():
    if connected_nodes_distances[f'pair {p}'] + path[node][0] < path[key_list[val_list.index(connected_nodes[f'pair {p}'][0])]][0]:
    path_extra[key_list[val_list.index(connected_nodes[f'pair {p}'][0])]] = [connected_nodes_distances[f'pair {p}'] + path[node][0], node]
    else:
    path_extra[key_list[val_list.index(connected_nodes[f'pair {p}'][0])]] = [connected_nodes_distances[f'pair {p}'] + path[node][0], node]

    p += 1
    p = 1

    print(path)

    path_old[node] = path[node]
    path.update(path_extra)
    del path[node]
    path_extra.clear()
    path = { k: v for k, v in sorted(path.items(), key = lambda item: item[1][0]) }

    path_old.update(path)

    ######################################################################## Section 4: Making a Path (with Coordinates) #######################################################

    final_node = f'node {len(nodes) - 1}'

    node_path = [final_node]

    while 'node 0' not in node_path:
    node_path.insert(0, path_old[node_path[0]][1])

    coordinate_path = []

    for node in node_path:
    coordinate_path.append(nodes[node])

    print(coordinate_path)

    ############################################################################### Section 5: Coloring the Path ###############################################################

    for coordinate in coordinate_path:
    maze.putpixel(coordinate, (255, 0, 0))

    c = 0
    h = 1
    v = 1

    while c<len(coordinate_path) - 1:

    if (coordinate_path[c + 1][1] + coordinate_path[c + 1][0]) - (coordinate_path[c][1] + coordinate_path[c][0]) > 0:
    h_dist = coordinate_path[c + 1][0] - coordinate_path[c][0]
    v_dist = coordinate_path[c + 1][1] - coordinate_path[c][1]

    while h<h_dist:
    new_coordinate = [coordinate_path[c][0] + h, coordinate_path[c][1]]
    maze.putpixel(new_coordinate, (255, 0, 0))

    h += 1

    while v<v_dist:
    new_coordinate = [coordinate_path[c][0], coordinate_path[c][1] + v]
    maze.putpixel(new_coordinate, (255, 0, 0))

    v += 1

    if (coordinate_path[c][1] + coordinate_path[c][0]) - (coordinate_path[c + 1][1] + coordinate_path[c + 1][0]) > 0:
    h_dist = coordinate_path[c][0] - coordinate_path[c + 1][0]
    v_dist = coordinate_path[c][1] - coordinate_path[c + 1][1]

    while h<h_dist:
    new_coordinate = [coordinate_path[c + 1][0] + h, coordinate_path[c + 1][1]]
    maze.putpixel(new_coordinate, (255, 0, 0))

    h += 1

    while v<v_dist:
    new_coordinate = [coordinate_path[c + 1][0], coordinate_path[c + 1][1] + v]
    maze.putpixel(new_coordinate, (255, 0, 0))

    v += 1

    h = 1
    v = 1
    c += 1

    maze.show()
        */
}
