
#!/bin/bash -l
set -e

readonly APPLICATION_NAME=stream-machine
readonly APPLICATION_STACK_NAME=$APPLICATION_NAME-infrastructure
readonly DATABASE_STACK_NAME=$APPLICATION_NAME-database
readonly FRONTEND_DEPLOY_BUCKET_NAME=app-$APPLICATION_NAME
readonly EB_SERVICE_ROLE_NAME=eb-role-stream-machine
readonly EB_DEPLOY_BUCKET_NAME=eb-stream-machine-deploy
readonly ECR_REPOSITORY_NAME=$APPLICATION_NAME

echo "Deploying application database"
aws cloudformation deploy --stack-name $DATABASE_STACK_NAME --template-file ./cloudformation-templates/application-database.yaml --parameter-overrides ApplicationName=$APPLICATION_NAME --capabilities CAPABILITY_NAMED_IAM

echo "Deploying application infrastructure"
aws cloudformation deploy --stack-name $APPLICATION_STACK_NAME --template-file ./cloudformation-templates/application-infrastructure.yaml --parameter-overrides ApplicationName=$APPLICATION_NAME FrontendBucketName=$FRONTEND_DEPLOY_BUCKET_NAME EbDeploymentBucketName=$EB_DEPLOY_BUCKET_NAME ContainerRepositoryName=$ECR_REPOSITORY_NAME --capabilities CAPABILITY_NAMED_IAM