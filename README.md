# Game stream search
http://app-stream-machine.s3-website-ap-southeast-2.amazonaws.com/

### Requirements to run
* Docker v2
* AWS Credentials for the account

### Requirements to develop
* Lerna v3.22+ `npm install --global lerna`
* Npm 6.14+
* .Net Core 3.1
* AWS Credentials for the account

### Commands
* `lerna bootstrap` - Install all dependencies
* `lerna run build` - Build all components
* `lerna run start:service --stream` - Builds and starts the backend service inside a docker container
* `lerna run stop:service --stream` - Stops the running service
* `lerna run test --stream` - run all unit tests
* `lerna run test:integration --stream` - run the integration tests (web service must be running first)

### How to run the application
1. Clone to repository to your local machine
2. Ensure that you have configured AWS authentication
3. Change the to folder into which the repository was cloned.
4. Run the application `docker-compose up`
5. Navigate your browser to http://localhost:8080
6. Stop the application `docker-compose down`

### How to develop the application
1. Clone to repository to your local machine
2. Ensure that you have configured AWS authentication on your local machine
3. Change the to folder into which the repository was cloned.
4. Install all dependencies `lerna bootstrap`
5. Start the web service and web server `lerna run start:service --stream`
6. Start the web client `lerna run start:local`
7. Navigate your browser to http://localhost:8080
8. Stop the web service `lerna run stop:service`

### Deploy AWS infrastructure needed to support the application
1. Change to the folder /devops
2. Deploy infrastructure by running ```deploy-application-infrastructure.sh```

### Deploy application to AWS
1. Change to the folder /devops
2. Deploy the application by running ```deploy-application-components.sh <env name> <build no>``` e.g ./deploy-application-components.sh prodA 123

### Future Improvements
#### Application
* Provide a way to discover new streamers
* Filter by langauge

#### Technical
* Setup a CICD build pipeline
* CloudFront CDN integration
* Telemtry and service monitoring
* Error logging frontend and backend
* E2E UI Tests
* Blue-Gree deployment
