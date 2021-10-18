/*********************************************
 * OPL 20.1.0.0 Model
 * Author: Adam McGregor
 * Creation Date: 17 Oct 2021 at 3:06:11 am
 *********************************************/

//constant to stay how many cities there are
int N = 15;
//constant for number of cities to choose
int K = 6;

// C -> range of cities
range C = 1..N;

//get the data from the sheet using .dat file
int Cities[C][C] = ...;

//decision vars; should this city be chosen
dvar boolean x[C];


//Objective function
minimize sum(i, j in C) Cities[i][j] * x[i];


//Constraints
subject to
{
  //pick K cities
  sum(i in C) x[i] == K;
  
  //forall (i in V)
  //   sum(j in V) x[i][j] <= 1; // (2)
}