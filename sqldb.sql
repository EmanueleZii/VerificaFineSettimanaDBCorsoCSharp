create Database Agenzia;
use Agenzia;

CREATE TABLE utenti (
    id INT PRIMARY KEY AUTO_INCREMENT,
    username varchar(255) UNIQUE NOT NULL,
    email varchar(255) UNIQUE NOT NULL,
    password varchar(255) NOT NULL,
    ruolo ENUM('admin', 'utente') NOT NULL DEFAULT 'utente'
);

CREATE TABLE luoghi (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nome varchar(255) NOT NULL,
    descrizione varchar(255)
);

CREATE TABLE attrazioni (
    id INT PRIMARY KEY AUTO_INCREMENT,
    luogo_id INTEGER NOT NULL,
    nome varchar(255) NOT NULL,
    descrizione varchar(255),
    FOREIGN KEY(luogo_id) REFERENCES luoghi(id)
);

INSERT INTO utenti (username, email, password, ruolo)
VALUES ('admin', 'admin@admin.com', 'admin', 'admin');