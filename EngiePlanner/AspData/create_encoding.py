import json
import datetime
import numpy as np
import os
import math

class JsonInputReader():
    def __init__(self, json_file, availability_file):
    
        self.file_name = json_file
        f=open(json_file,'r')
        self.json_data = json.load(f)
        f.close()
        self.availability_file_name = availability_file
        f = open(availability_file, 'r')
        self.availability_data = json.load(f)
        f.close()
        if not 'tasks' in self.json_data.keys():
            raise ValueError(self.file_name + ' does not contain "tasks" object!')
        
        self.integer_dates = {}
        self.integer_week_day = {}
        self._determin_integer_date()
        
        self.lp = self._transform_json2lp()
        
    def _transform_json2lp(self, max_days = 'default'):
        '''
    _transform_jason2lp transforms the dictionary read from the problem instance json, and returns the raw 
    text of the logic program instance.
    
    :max_days:  options for the number of days in the logic program. the max number of days must be a multiple of five, to 
                avoid inconsitencies with the number of weeks. To ensure this, _transform_json2lp will add days if necessary. 
                'default' -> integer day of latest json date + 5 days is the last day in the lp  (recomended)
                'min' -> integer day of latest json date is the last day in the lp
                int -> select the last integer day of the program. value must be greater than 1
        '''
                                
        lp = '''%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Problem Instance 
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n'''
        lp_complex = '\n'
        jobs = set()
        deliveries = set()
        for task in self.json_data['tasks']:
            if task['duration'] <= 38:
                lp += 'task_available_day({}, {}).\n'.format(task['name'], self.integer_dates[task['start_date']])
                available_week = int(np.ceil(self.integer_dates[task['start_date']]/5))
                lp += 'task_available_week({}, {}).\n'.format(task['name'], available_week)
                lp += 'task_planned_day({}, {}).\n'.format(task['name'], self.integer_dates[task['planned_date']])
                planned_week = int(np.ceil(self.integer_dates[task['planned_date']]/5))
                lp += 'task_planned_week({}, {}).\n'.format(task['name'], planned_week)
                lp += 'task_duration({}, {}).\n'.format(task['name'], task['duration'])
                
                if not task['name'] in jobs:
                    lp += 'job_duration({}, {}).\n'.format(task['name'], task['duration'])
                jobs.add(task['name'])
                #if not task['delivery'] in deliveries:
                #    lp += 'delivery({}).\n'.format(task['delivery'])
                #deliveries.add(task['delivery'])
                lp += '\n'
                #if task['subteam']:
                #    lp_complex += 'capable_of(E,{}) :- sub_team_member(E,team1).\n'.format(task['name'], task['subteam'])
                if task['employees']:
                    for empl in task['employees']:
                        lp_complex += 'capable_of({},{}).\n'.format(empl, task['name'])
                if task['predecessors']:
                    for predecessor in task['predecessors']:
                        lp_complex += 'predecessor({},{}).\n'.format(predecessor, task['name'])
                

        if max_days == 'default':
            last_day = int(max(self.integer_dates.values()))+5
            last_day = last_day + 5-last_day%5
            lp_complex += '\ndays(1..{}).\n'.format(last_day)            
            lp_complex += 'weeks(1..{}).\n'.format(int(np.ceil(last_day/5))) 
        elif max_days == 'min':
            last_day = int(max(self.integer_dates.values()))
            last_day = last_day + 5-last_day%5
            lp_complex += '\ndays(1..{}).\n'.format(last_day)            
            lp_complex += 'weeks(1..{}).\n'.format(int(np.ceil(last_day/5))) 
        elif isinstance(max_days, int) and max_days > 1:
            last_day = max_days
            last_day = last_day + 5-last_day%5
            lp_complex += '\ndays(1..{}).\n'.format(last_day)            
            lp_complex += 'weeks(1..{}).\n'.format(int(np.ceil(last_day/5))) 
        else:
            raise ValueError('JsonInputReader._transform_json2lp: max_days must be "dafault", "min" or integer value>1!')
        for job in jobs:
            lp_complex += 'job({}).\n'.format(job)
        
        first_day = min(self.integer_week_day)
        week_day = self.integer_week_day[first_day]
        for day in range(first_day, last_day):
            if week_day > 4:
                lp_complex += 'weekend({}).\n'.format(day)
            week_day += 1
            if week_day == 7:
                week_day = 0
        index = 0
        week = 1
        lp += '\n'
        for availability in self.availability_data:
            if availability['name'] != self.availability_data[index-1]['name']:
                week = 1
            else:
                week += 1
            lp += 'capacity({}, {}, {}). '.format(availability['name'], week, math.floor(availability['availableHours']))
            lp += '\n'
            index += 1
        return(lp+lp_complex)
            
    def _determin_integer_date(self):
        dates_raw = []
        dates = []
        
        for task in self.json_data['tasks']:
            dates_raw.append(task['start_date'])
            dates_raw.append(task['planned_date'])

        # for availability in self.availability_data:
        #     dates_raw.append(availability['fromDate'])
        #     dates_raw.append((availability['toDate']))

        for i in dates_raw:
            dates.append(datetime.datetime.strptime(i, '%d.%m.%Y'))
            
        day1 = min(dates)
        dates.remove(day1)
        
        self.integer_dates.update({day1.strftime('%d.%m.%Y')  : 1})
        self.integer_week_day.update({1 : day1.weekday()})
        
        for i in dates:
            delta = i-day1
            self.integer_dates.update({i.strftime('%d.%m.%Y'): delta.days+1})
            self.integer_week_day.update({delta.days+1 : i.weekday()})
        
        
    def write_lp(self, encoding_file):
        dirname = os.path.dirname(__file__)
        out_file = os.path.join(dirname, 'encoding.lp4').replace('\\', '/')
        encoding = None
        with open(encoding_file, 'r') as f:
            encoding = f.read()
        
        with open(out_file, 'w') as out:
            out.write(self.lp)
            out.write(encoding)
            
class JsonOutputWriter():
    def __init__(self, answer_set, date2integer_dict, output_file_name = 'clingo_output.json'):
        self.answer_set = answer_set
        self.output_file_name = output_file_name
        self.date2integer_dict = date2integer_dict
        
        dates = []
        
        for i in self.date2integer_dict:
            dates.append(datetime.datetime.strptime(i, '%d.%m.%Y'))
        self.schedule_start_day = min(dates)
        
        self.answer_set_list = []
        self.answer_set_list_start_end = []
        self.json_string = ''
        
        self._start_end_dict()

        dirname = os.path.dirname(__file__)
        output = os.path.join(dirname, 'output.json').replace('\\', '/')
        self._write_json(self.answer_set_list_start_end, output)

    def _start_end_dict(self):
        temp_answer_set = self.answer_set.copy()
        for atom in self.answer_set:
            if 'start_day' in atom:
                start_day = atom
                start_day_items = start_day[:-1].split('(')[1].split(',')
                for atom2 in temp_answer_set:

                    if 'end_day' in atom2:
                        end_day = atom2
                        end_day_items = end_day[:-1].split('(')[1].split(',')
                        if start_day_items[2] == end_day_items[2]:
                            temp_day_start = self.schedule_start_day + datetime.timedelta(days=int(start_day_items[1]))
                            temp_day_end = self.schedule_start_day + datetime.timedelta(days=int(end_day_items[1]))
                            temp_date_start = temp_day_start.strftime('%d.%m.%Y')
                            temp_date_end = temp_day_end.strftime('%d.%m.%Y')
                            temp_dict = {
                                            'start': temp_date_start,
                                            'finish': temp_date_end,
                                            'task': start_day_items[2]
                                            }
                         
                            self.answer_set_list_start_end.append(temp_dict)
                                        
    
    def _write_json(self,_list, output_file):
        json_string = json.dumps(_list)
        with open(output_file,'w') as f:
            json_string = json.dump(_list, f, indent = 4)
        
        
if __name__ == '__main__':
    dirname = os.path.dirname(__file__)
    inputFile = os.path.join(dirname, 'tasks.json').replace('\\', '/')
    availabilityFile = os.path.join(dirname, 'availability.json').replace('\\', '/')
    template = os.path.join(dirname, 'template.lp4').replace('\\', '/')
    json = JsonInputReader(inputFile, availabilityFile)
    json.write_lp(encoding_file = template)
