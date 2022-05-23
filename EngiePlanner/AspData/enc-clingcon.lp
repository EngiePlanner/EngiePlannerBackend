% available employees
employee(E) :- capable_of(E,_).

% tasks to be scheduled
task(T) :- task_available_day(T,_).

% which days belong to which week
day_of_week(D,W) :- days(D), weeks(W), W = (D-1)/5 + 1.
first_day_of_week(D,W) :- day_of_week(D,W), D = #min { M : day_of_week(M,W) }. 
last_day_of_week(D,W) :- day_of_week(D,W), D = #max { M : day_of_week(M,W) }. 

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Feasible Solutions 
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% each task gets assigned one employee capable of doing it
{ asg_empl(T,E) : capable_of(E,T) } = 1 :- task(T).

% guess a daily effort for each task
{ effort(T,L) : L = 1..8 } = 1 :- task(T). 

% assign start and end dates to tasks 
&dom{ A..M } = start(T) :- task(T), task_planned_day(T,M), task_available_day(T,A).
&dom{ A..M } = end(T) :- task(T), task_planned_day(T,M), task_available_day(T,A).
:- task(T), &sum{ end(T); -start(T) } < 0.

% tasks must respect predecessor relation
:- predecessor(T1,T2), &sum{ start(T2); -end(T1) } < 1. 

% every task has a length
&dom{ 1..M  } = length(T) :- task(T), M = #max{ D: days(D) }.
:- task(T), &sum{ start(T); length(T); -end(T) } != 1.

% for each task, total effort must be greater than required effort
:- task_duration(T,H), effort(T,E), M = ((H-1) / E)+1, &sum { length(T) } != M.

% tasks are assigned to weeks
asg_first_week(T,W) :- task(T), weeks(W), day_of_week(D,W), &sum{ start(T) } = D.
asg_last_week(T,W) :- task(T), weeks(W), day_of_week(D,W), &sum{ end(T) } = D.
asg_week(T,W) :- asg_first_week(T,W).
asg_week(T,W) :- asg_week(T,W-1), W <= W1, asg_last_week(T,W1).

% weekly effort per task and employee
&dom{ 0..8*5 } = weekly_effort(T,W,EM) :- task(T), weeks(W), employee(EM).
:- asg_week(T,W), not asg_first_week(T,W), not asg_last_week(T,W), effort(T,E), S = E*5,
   &sum{ weekly_effort(T,W,EM) } != S, asg_empl(T,EM).
:- task(T), weeks(W), not asg_week(T,W), 
   &sum{ weekly_effort(T,W,EM) } > 0, asg_empl(T,EM).
:- asg_first_week(T,W), asg_last_week(T,W), task_duration(T,H), 
   &sum{ weekly_effort(T,W,EM) } != H, asg_empl(T,EM). 
:- asg_first_week(T,W), not asg_last_week(T,W), last_day_of_week(LD,W), effort(T,E),
   &sum { -start(T); LD; 1 } = DD, DD=1..5, S = E*DD, 
   &sum{ weekly_effort(T,W,EM) } != S, asg_empl(T,EM).
:- asg_last_week(T,W), not asg_first_week(T,W), first_day_of_week(FD,W), effort(T,E),
   &sum { end(T); -FD; 1 } = DD, DD=1..5, S = E*DD, 
   &sum{ weekly_effort(T,W,EM) } != S, asg_empl(T,EM).
:- task(T), weeks(W), employee(EM), not asg_empl(T,EM),
   &sum{ weekly_effort(T,W,EM) } > 0.

% weekly load per employee must not exceed capacity
:- employee(EM), weeks(W), capacity(E,W,C), &sum { weekly_effort(T,W,E) : task(T) } > C.

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Objectives
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% balance work load
total_load(E,M) :- employee(E),  M = #sum { H,T: asg_empl(T,E), task_duration(T,H) }.
min_max(D) :- MN = #min { M,E: total_load(E,M); 10000 },
              MX = #max { M,E: total_load(E,M); 0 }, D = MX-MN.
%&sum{ balance } = D*10000 :- min_max(D).
#minimize { D*10: min_max(D) }.

% minimize length of tasks
#minimize { 8-E,T: effort(T,E) }.

% minimize total completion time
%&minimize{ end(T) : task(T); balance }.
%end_day(T,D) :- task(T), days(D), &sum{ end(T) } = D.
%#minimize{ D@1,T: end_day(T,D) }.  


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Output 
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

#show asg_empl/2.
#show effort/2.
#show total_load/2.
&show{ start(T) : task(T) }.
&show{ end(T) : task(T) }.

% best solver configuration:
% clingcon --configuration=handy -t4
