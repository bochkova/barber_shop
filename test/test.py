import gevent
import json
import time
from locust import events, HttpUser, TaskSet, task, constant_pacing
from locust.runners import STATE_STOPPING, STATE_STOPPED, STATE_CLEANUP, MasterRunner, LocalRunner

class User(HttpUser):
    wait_time = constant_pacing(1)
    
    def on_start(self):
        self.client.verify = False
        self.client.headers.update({'Content-Type': 'application/json'})
        self.login()
        
    def login(self):
        response = self.client.post("/api/Auth/login",
            data=json.dumps({"username":"admin", "password":"admin"})
        ) 
        self.token = response.json()['token']
        self.client.headers.update({"Authorization": f"Bearer {self.token}"})
    
    @task
    def get_apointments(self):
        self.client.get("/api/Appointments")

    @task
    def get_customers(self):
        self.client.get("/api/Customers")

    @task
    def get_barbers(self):
        self.client.get("/api/Barbers")
    
    @task
    def customer_scenario(self):
        customercreatedata = {"name": "test_name", "preferredstyle": "test_style"}
        response = self.client.post("/api/customers", data=json.dumps(customercreatedata))
        if response.status_code != 201:
            return
        
        customerid = response.json()['id']
        response = self.client.get(f"/api/Customers/{customerid}")
        response = self.client.delete(f"/api/Customers/{customerid}")
        
        
    

        # customerchangedata = {"name": "new_test_name"}
        # response = self.client.put(f"/api/Customers/{customerid}", data=json.dumps(customerchangedata))
                
    
    
def checker(environment):
    while environment.runner.state not in [STATE_STOPPING, STATE_STOPPED, STATE_CLEANUP]:
        time.sleep(1)
        if environment.runner.stats.total.fail_ratio > 0.1:
            print(f"fail ratio was {environment.runner.stats.total.fail_ratio}, quitting")
            environment.runner.quit()
            return

@events.init.add_listener
def on_locust_init(environment, **_kwargs):
    if isinstance(environment.runner, MasterRunner) or isinstance(environment.runner, LocalRunner):
        gevent.spawn(checker, environment)