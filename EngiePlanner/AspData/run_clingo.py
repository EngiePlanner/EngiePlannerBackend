import os
from clingo import Control
from clingo.ast import parse_string, ProgramBuilder
from clingodl import ClingoDLTheory
from create_encoding import JsonOutputWriter as jow
from create_encoding import JsonInputReader as jir
import time
import logging


def one_clingodl_run(prg, n_solution=0, time_limit=None):
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
            models.append(mdl.symbols(atoms=True))
            dl.append([f'dl({key},{val})' for key, val in thy.assignment(mdl.thread_id)])
            t_temp = time.time()
            #break loop if time limit is reached
            if time_limit != None:
                if t_temp - t_start > time_limit:
                    print('time limit reached!\n\n')
                    break
    
    #models[-1] is the optimal solution
    temp_mdl = []
    for i in models[-1]:
        temp_mdl.append(str(i))
    temp_mdl += dl[-1]
    
    return(temp_mdl)


if __name__ == '__main__':
    dirname = os.path.dirname(__file__)
    inputFile = os.path.join(dirname, 'tasks.json').replace('\\', '/')
    availabilityFile = os.path.join(dirname, 'availability.json').replace('\\', '/')
    template = os.path.join(dirname, 'template.lp4').replace('\\', '/')
    taskMasterAutogen = os.path.join(dirname, 'encoding.lp4').replace('\\', '/')
    json = jir(inputFile, availabilityFile)
    json.write_lp(encoding_file = template)
        
    lp = ''
    with open(taskMasterAutogen, 'r') as f:
        lp = f.read()

    answer = one_clingodl_run(lp, n_solution=0,  time_limit = 15)
    output_json = jow(answer, json.integer_dates)
