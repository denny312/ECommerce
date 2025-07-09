-- Database MySQL per e-commerce rasoi
-- Migrato da PostgreSQL

-- Crea il database (opzionale)
-- CREATE DATABASE ecommerce;
-- USE ecommerce;

-- Tabelle
CREATE TABLE utenti (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100),
    email VARCHAR(100) UNIQUE,
    password VARCHAR(100)
);

CREATE TABLE rasoi (
    id INT AUTO_INCREMENT PRIMARY KEY,
    marca VARCHAR(100),
    modello VARCHAR(100),
    prezzo DECIMAL(6, 2),
    tipo VARCHAR(50)  -- es: "elettrico", "manuale"
);

CREATE TABLE recensioni (
    id INT AUTO_INCREMENT PRIMARY KEY,
    utente_id INT,
    rasoio_id INT,
    voto INT CHECK (voto >= 1 AND voto <= 5),
    commento TEXT,
    FOREIGN KEY (utente_id) REFERENCES utenti(id) ON DELETE CASCADE,
    FOREIGN KEY (rasoio_id) REFERENCES rasoi(id) ON DELETE CASCADE
);

CREATE TABLE ordini (
    id INT AUTO_INCREMENT PRIMARY KEY,
    utente_id INT,
    data TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (utente_id) REFERENCES utenti(id)
);

CREATE TABLE ordini_rasoi (
    ordine_id INT,
    rasoio_id INT,
    quantita INT DEFAULT 1,
    PRIMARY KEY (ordine_id, rasoio_id),
    FOREIGN KEY (ordine_id) REFERENCES ordini(id) ON DELETE CASCADE,
    FOREIGN KEY (rasoio_id) REFERENCES rasoi(id) ON DELETE CASCADE
);

-- Dati
INSERT INTO utenti (nome, email, password) VALUES
('Mario Rossi', 'mario@example.com', 'pass1'),
('Luigi Bianchi', 'luigi@example.com', 'pass2'),
('Giulia Verdi', 'giulia@example.com', 'pass3'),
('Anna Neri', 'anna@example.com', 'pass4'),
('Pietro Blu', 'pietro@example.com', 'pass5');

INSERT INTO rasoi (marca, modello, prezzo, tipo) VALUES
('Gillette', 'Fusion5', 12.99, 'manuale'),
('Philips', 'OneBlade', 34.99, 'elettrico'),
('Braun', 'Series 7', 129.99, 'elettrico'),
('Wilkinson', 'Hydro 5', 9.49, 'manuale'),
('Panasonic', 'ES-LV65', 179.99, 'elettrico');

INSERT INTO recensioni (utente_id, rasoio_id, voto, commento) VALUES
(1, 1, 4, 'Buon prodotto!'),
(2, 2, 5, 'Fantastico!'),
(3, 3, 3, 'Nella media'),
(4, 4, 4, 'Ottimo rapporto qualità/prezzo'),
(5, 5, 5, 'Il migliore mai provato!');

INSERT INTO ordini (utente_id) VALUES
(1), (2), (3), (4), (5);

INSERT INTO ordini_rasoi (ordine_id, rasoio_id, quantita) VALUES
(1, 1, 2),
(1, 2, 1),
(2, 3, 1),
(3, 4, 3),
(4, 5, 1);
