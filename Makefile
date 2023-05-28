COMPOSE_FILE := .\docker-compose-dev.yml
CONTAINER_NAME := gal

up:
	setx BUILDKIT 1
	docker-compose -f $(COMPOSE_FILE) up -d --build

dn:
	docker compose -f $(COMPOSE_FILE) down

