# Game stream search
Allows searching for game streams across multi platforms.  Supported platforms are Twitch, YouTube and DLive  
http://app-stream-machine.s3-website-ap-southeast-2.amazonaws.com/

### Requirements to run
* Docker v2
* AWS Credentials for the account

### Requirements to develop
* Lerna v3.22+ `npm install --global lerna`
* Node 14+
* Npm 6.14+
* .Net Core 5.0
* AWS Credentials for the account

### Commands
* `lerna bootstrap` - Install all dependencies
* `lerna run build` - Build all components
* `lerna run start:service --stream` - Builds and starts the backend service inside a docker container
* `lerna run stop:service --stream` - Stops the running service
* `lerna run test --stream` - run all unit tests
* `lerna run test:integration --stream` - run the integration tests (web service must be running first)
* `lerna run lint` - run the linter over the frontend code

### How to run the application with docker
1. Clone to repository to your local machine
2. Change to the folder into which the repository was cloned.
3. In the application root folder, create the file .env file containing your AWS credentials as follows:  
     `AWS_ACCESS_KEY_ID=xxxx`  
     `AWS_SECRET_ACCESS_KEY=xxxx`
4. Run the application `docker-compose up`
5. Navigate your browser to http://localhost:8080
6. Stop the application `docker-compose down`

### How to develop the application
1. Clone to repository to your local machine
2. Change the to folder into which the repository was cloned.
3. Install all dependencies `lerna bootstrap`
4. In the application root folder, create the file .env file containing your AWS credentials as follows:  
     `AWS_ACCESS_KEY_ID=xxxx`  
     `AWS_SECRET_ACCESS_KEY=xxxx`
5. Start the web service and web server `lerna run start:service --stream`
6. Start the web client `lerna run start:local`
7. Navigate your browser to http://localhost:8080
8. Stop the web service `lerna run stop:service`

### How to run the service through Visual Studio
1. Clone to repository to your local machine
2. Change the to folder into which the repository was cloned.
3. Install all dependencies `lerna bootstrap`
4. Ensure that you have configured the AWS credentials using `aws configuration`
5. Open the service in Visual Studio and run the application
6. Start the web client `lerna run start:local`
7. Navigate your browser to http://localhost:8080

### Deploy AWS infrastructure needed to support the application
1. Change to the folder /devops
2. Deploy infrastructure by running ```deploy-application-infrastructure.sh```

### Deploy application to AWS
1. Change to the folder /devops
2. Deploy the application by running ```deploy-application-components.sh <env name> <build no>``` e.g ./deploy-application-components.sh prodA 123

### Architecture
Onion + Clean Architecture

![alt text](https://github.com/kerryconvery/game-stream-search/blob/master/documentation/Game-Stream-Search-Architecture.png?raw=true)

### Infrastructure
* Cloud Provider: AWS
* Frontend hosting: AWS S3 bucket static website
* Service hosting: AWS Elastic Beanstalk
* Container repository: AWS Elastic Container Repository
* Secrets storage: AWS Systems manager paramater store with KMS encryption

### Future Improvements
#### Application
* Provide a way to discover new streamers (in progress)
* Filter by langauge

#### Technical
* Setup a CICD build pipeline
* Change frontend hosting to AWS CloudFront
* Telemtry and service monitoring
* Error logging frontend and backend
* E2E UI Tests
* Blue-Green deployment
