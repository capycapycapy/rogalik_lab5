using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.Start();
    }
}

class Game
{
    private Random rnd = new Random();
    private Player player;
    private List<Enemy> enemies;
    private List<Weapon> weapons;
    private Aid currentAid;
    private int score;

    public void Start()
    {
        Console.WriteLine("Добро пожаловать, воин!");
        Console.Write("Назови себя: ");
        string name = Console.ReadLine();
        player = new Player(name);

        InitializeWeapons();
        InitializeEnemies();

        Console.WriteLine($"Ваше имя {player.Name}!");
        Console.WriteLine($"В мусорке вы нашли {player.Weapon.Name}!");
        Console.WriteLine($"У вас {player.MaxHealth}hp.");

        Play();
    }

    private void InitializeWeapons()
    {
        weapons = new List<Weapon>
        {
            new Weapon("Дубина переговоров", 20, 5),// имя, урон, кол-во раундов с оружием.
            new Weapon("Элитная бита первопроходца", 15, 3),
            new Weapon("Карты таро", 10, 2)
        };
        player.Weapon = weapons[rnd.Next(weapons.Count)];
    }

    private void InitializeEnemies()
    {
        enemies = new List<Enemy>
        {
            new Enemy("Страж света", 50, new Weapon("Экскалибур", 10, 3)),
            new Enemy("Фантазм мутных вод", 40, new Weapon("Дубина", 8, 2)),
            new Enemy("Хиличурл", 70, new Weapon("Деревяная мотыга", 12, 1)),
            new Enemy("Мужчина", 60, new Weapon("Пиво", 9, 2)),
            new Enemy("Моунш дизайнер", 30, new Weapon("Афтер эффектс", 5, 5))
        };
    }

    private void Play()
    {
        foreach (var enemy in enemies)
        {
            Console.WriteLine($"\n{player.Name} встречает врага {enemy.Name} ({enemy.Health}hp), Враг имеет оружие {enemy.Weapon.Name} ({enemy.Weapon.Damage})");
            currentAid = GetRandomAid(); //случайная аптечка в начале раунда
            Console.WriteLine($"Вы нашли аптечку: {currentAid.Name} (восстановление {currentAid.HealthRestore}hp).");

            bool aidUsed = false; //отслеживания аптечки

            while (player.Health > 0 && enemy.Health > 0)
            {
                Console.WriteLine("Что вы будете делать?");
                Console.WriteLine("1. Ударить");
                Console.WriteLine("2. Пропустить ход");
                Console.WriteLine("3. Использовать аптечку");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Attack(enemy);
                        break;
                    case "2":
                        Console.WriteLine($"{player.Name} пропустил ход.");
                        break;
                    case "3":
                        if (!aidUsed)
                        {
                            UseAid();
                            aidUsed = true; //аптечка использована
                        }
                        else
                        {
                            Console.WriteLine("Вы уже использовали аптечку в этом раунде.");
                        }
                        break;
                    default:
                        Console.WriteLine("НЕВЕРНЫЙ ВЫБОР!! WARNING WARNING! SOS. повторите попытку");
                        break;
                }

                if (enemy.Health > 0)
                {
                    enemy.Attack(player);
                }
            }

            if (player.Health <= 0)
            {
                Console.WriteLine("Вы погибли. Игра окончена.");
                return;
            }
            else
            {
                Console.WriteLine($"Вы победили {enemy.Name}!");
                score += 10;
                Console.WriteLine($"Ваши очки: {score}");
            }
        }

        Console.WriteLine("Вы победили всех врагов! Игра окончена.");
    }

    private Aid GetRandomAid()
    {
        List<Aid> aids = new List<Aid>
        {
            new Aid("Маленькая аптечка", 10),
            new Aid("Средняя аптечка", 20),
            new Aid("Большая аптечка", 30)
        };
        return aids[rnd.Next(aids.Count)];
    }

    public void Attack(Enemy enemy)
    {
        Console.WriteLine($"{player.Name} ударил противника {enemy.Name}.");
        enemy.Health -= player.Weapon.Damage;

        if (enemy.Health <= 0)
        {
            Console.WriteLine($"{enemy.Name} повержен.");
            player.Weapon.Durability--;
            if (player.Weapon.Durability <= 0)
            {
                Console.WriteLine($"{player.Weapon.Name} сломался!");
                player.Weapon = weapons[rnd.Next(weapons.Count)];
                Console.WriteLine($"Вы подобрали новое оружие: {player.Weapon.Name}.");
            }
        }
        else
        {
            Console.WriteLine($"У противника {enemy.Name} {enemy.Health}hp, у вас {player.Health}hp");
        }
    }

    private void UseAid()
    {
        if (player.Health < player.MaxHealth)
        {
            player.Health += currentAid.HealthRestore;
            if (player.Health > player.MaxHealth)
            {
                player.Health = player.MaxHealth;
            }
            Console.WriteLine($"{player.Name} использовал аптечку {currentAid.Name}. У вас {player.Health}hp.");
        }
        else
        {
            Console.WriteLine("Вы не можете использовать аптечку, у вас полное здоровье.");
        }
    }
}

class Player
{
    public string Name { get; }
    public int Health { get; set; }
    public int MaxHealth { get; } = 100;
    public Weapon Weapon { get; set; }

    public Player(string name)
    {
        Name = name;
        Health = MaxHealth;
    }
}

class Enemy
{
    public string Name { get; }
    public int Health { get; set; }
    public Weapon Weapon { get; }

    public Enemy(string name, int health, Weapon weapon)
    {
        Name = name;
        Health = health;
        Weapon = weapon;
    }

    public void Attack(Player player)
    {
        Console.WriteLine($"{Name} ударил вас!");
        player.Health -= Weapon.Damage;
        Console.WriteLine($"У противника {Name} {Health}hp, у вас {player.Health}hp");
    }
}


class Aid
{
    public string Name { get; }
    public int HealthRestore { get; }

    public Aid(string name, int healthRestore)
    {
        Name = name;
        HealthRestore = healthRestore;
    }
}

class Weapon
{
    public string Name { get; }
    public int Damage { get; }
    public int Durability { get; set; }

    public Weapon(string name, int damage, int durability)
    {
        Name = name;
        Damage = damage;
        Durability = durability;
    }
}