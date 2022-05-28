import os

from clingo import Control
from clingo.ast import parse_string, ProgramBuilder
from clingodl import ClingoDLTheory
import datetime
from pot_plot_json_io import JsonOutputWriter as jow
from pot_plot_json_io import JsonInputReader as jir
import time

def one_clingodl_run(prg, n_solution = 0, time_limit = None):
    lp = prg
    #running clingo
    thy = ClingoDLTheory()
    ctl = Control([str(n_solution)])
    thy.register(ctl)
    with ProgramBuilder(ctl) as bld:
        parse_string(lp, lambda ast: thy.rewrite_ast(ast, bld.add))

    ctl.ground([('base', [])])
    thy.prepare(ctl)
        
    models = []
    dl = []

    #solve and extract all true terms of each model
    with ctl.solve(async_ = True, yield_=True, on_model=thy.on_model) as hnd:
        t_start = time.time()
        for mdl in hnd:
            #the cost of each model can be seen via mdl.cost
            #print(mdl.cost)
            models.append(mdl.symbols(atoms=True))
            dl.append([f'dl({key},{val})' for key, val in thy.assignment(mdl.thread_id)])
            t_temp = time.time()
            #break loop if time limit is reached
            if time_limit != None:
                if t_temp - t_start > time_limit:
                    print('time limit reached!\n\n')
                    break#
    
    #models[-1] is the optimal solution
    #
    temp_mdl = []
    for i in models[-1]:
        temp_mdl.append(str(i))
    temp_mdl += dl[-1]
    
    return(temp_mdl)

# def reshape_date_start(date):
#     d,m,y = date.split('.')
#     dt = datetime.datetime.strptime(date, '%d.%m.%Y') + datetime.timedelta(hours = 8)
#     #return('{}-{}-{}'.format(y,m,d))
#     return(dt)
#
# def reshape_date_finish(date):
#     d,m,y = date.split('.')
#     dt = datetime.datetime.strptime(date, '%d.%m.%Y') + datetime.timedelta(hours = 17)
#     #return('{}-{}-{}'.format(y,m,d))
#     return(dt)


if __name__ == '__main__':
    dirname = os.path.dirname(__file__)
    inputFile = os.path.join(dirname, 'tasks.json').replace('\\', '/')
    availabilityFile = os.path.join(dirname, 'availability.json').replace('\\', '/')
    taskMasterAutogenEncoding = os.path.join(dirname, 'task_master_pot_encoding.lp4').replace('\\', '/')
    taskMasterAutogen = os.path.join(dirname, 'task_master_autogen.lp4').replace('\\', '/')
    json = jir('tasks.json', 'availability.json')
    json.write_lp(encoding_file = 'task_master_pot_encoding.lp4')
        
    lp = ''
    with open('task_master_autogen.lp4', 'r') as f:
        lp = f.read()

    answer = one_clingodl_run(lp, n_solution=0,  time_limit = 15)
    #print(answer)
    output_json = jow(answer, json.integer_dates)

    #df = pd.read_json('start_end.json')
    #df.Start = df.Start.apply(reshape_date_start)
    #df.Finish = df.Finish.apply(reshape_date_finish)
    
    #fig = px.timeline(df, x_start="Start", x_end="Finish", y="Resource", color="Task")
    #fig.show()
    
    #fig = ff.create_gantt(df=df, index_col = 'Task', show_colorbar=True )
    #fig.update_yaxes(autorange="reversed") # otherwise tasks are listed from the bottom up
    #fig.show()
    
    #embed()
