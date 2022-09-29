## Optimal Placement of Market Areas on Fantasy Town Maps

| Class Detail    | Information |
| --------------- | ----------- |
| `Course`        | CSCI 716 - Computational Geometry |
| `Team Members`  | Brock Dyer  |
| `Category`      | 3 - Applications in other fields |


## Description:
Find an ideal set of n market locations within a region that optimize customer utility using weighted voronoi diagrams and metaheuristic search.

There are many digital tools to generate medieval town maps for fantasy worlds (my favorite being https://watabou.itch.io/medieval-fantasy-city-generator). These tools dramatically speed up the world building process for people that are lacking inspiration or artistic talent. The tool linked above includes some options for customizing the generator, such as the presence of rivers, the number of roads, if there is a castle and walls, etc... However, these tools do not provide an entire town. Although a pretty map is usually most of the battle for world-builders, it would be nice to have generators that can place markets, inns, and other necessities.

Voronoi diagrams are useful tools for subdividing a plane around focal points such that the cell region surrounding a centroid contains the points that are closer to that point than any other point. Consider a town with three markets. The regions of influence each market has could be approximated by a voronoi diagram using the market locations as centroids. This would be a very rough approximation because it would not account for various factors such as wealth distributions, the market’s size and power, and street distance to travel. The goal of this project is to optimize market placement based on such considerations.

Given an initial set of n market locations, use a voronoi diagram to approximate regions of influence. Then sample customer points within the influence region and evaluate a fitness function for those customers. This then becomes the basis of a search problem in which the best n market locations based on the fitness function is the goal of the search. Metaheuristic optimization algorithms such as the firefly algorithm have been shown to find sufficient solutions to these complex search problems in a tractable amount of time.

## References:
- Dr. K. Umadevi, Optimizing Rural Dealers location – A Voronoi Approach. International
Journal of Civil Engineering and Technology, 9(8), 2018, pp. 879-886. http://
www.iaeme.com/IJCIET/issues.asp?JType=IJCIET&VType=9&IType=8
- Wang Wei, Feng Xuejun and Huang Li, "Research on regional logistics system layout
 optimization based on weighted voronoi diagram and gravitational model," 2008 IEEE International Conference on Automation and Logistics, 2008, pp. 2078-2083, doi: 10.1109/ ICAL.2008.4636506.
- Yang XS (2008) Nature-inspired metaheuristic algorithms. Luniver Press, Beckington
- Yushimito, W.F., Jaller, M. & Ukkusuri, S. A Voronoi-Based Heuristic Algorithm for Locating
Distribution Centers in Disasters. Netw Spat Econ 12, 21–39 (2012). https://doi.org/ 10.1007/s11067-010-9140-9

## Timeline:
- (2 weeks) Implement computation of weighted voronoi diagrams / delaunay triangulation - (2 week) Design and Implement an algorithm to sample points within a voronoi cell.
  - Needs to be uniform, and representative of the cell.
- (2 week) Design a fitness function to evaluate the utility of a given solution state.
- (2 week) Implement the Firefly algorithm (or another PSO optimization algorithm).
- (2 weeks) Develop the webpage to present results.

Stretch goals: read in a map and locate the markets on the map, sample from houses on map.
