from __future__ import print_function

import json
import urllib
import boto3
import os
import decimal
from decimal import *

# Helper class to convert a DynamoDB item to JSON.
class DecimalEncoder(json.JSONEncoder):
    def default(self, o):
        if isinstance(o, decimal.Decimal):
            if o % 1 > 0:
                return float(o)
            else:
                return int(o)
        return super(DecimalEncoder, self).default(o)

print('Loading function')

dynamodb = boto3.resource('dynamodb',region_name='us-east-1')
s3 = boto3.client('s3')


def updateDynamoDB(table, response):
	newMetadata = table.put_item(
		Item={
			'Client_id': response['Metadata']['client_id'],
			'Game_name': response['Metadata']['game_name'],
        	'Game_string': response['Metadata']['game_string'],
        	'Player_name': response['Metadata']['player_name'],
        	'Sdk_version': Decimal(response['Metadata']['sdk_version']),
        	'Unity_version': Decimal(response['Metadata']['unity_version']),
        	'Video_id': int(response['Metadata']['video_id'])
    	}
	)

	print("PutItem succeeded:")
	print(json.dumps(newMetadata, indent=4, cls=DecimalEncoder))

def get_bucket_key(event):
    message = event['Records'][0]['Sns']['Message']
    message_json = json.loads(message)
    bucket = message_json['Records'][0]['s3']['bucket']['name'].encode('utf8')
    key = message_json['Records'][0]['s3']['object']['key'].encode('utf8')
    return bucket, key

def lambda_handler(event, context):

    # Get the bucket and key from event message
    bucket, key = get_bucket_key(event)
    
    # Get object details from bucket and key
    response = s3.get_object(Bucket=bucket, Key=key)
    
    # Calls function to update metadata
    table = dynamodb.Table('VideoMetadata')
    updateDynamoDB(table, response)