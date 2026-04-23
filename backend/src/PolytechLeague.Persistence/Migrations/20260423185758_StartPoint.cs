using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StartPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:armwrestling_category", "men,women,open")
                .Annotation("Npgsql:Enum:armwrestling_hand", "left,right,both")
                .Annotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
                .Annotation("Npgsql:Enum:badminton_tournament_type", "single_elimination,double_elimination,round_robin,groups_playoff,swiss")
                .Annotation("Npgsql:Enum:basketball3x3tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .Annotation("Npgsql:Enum:beach_volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .Annotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .Annotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .Annotation("Npgsql:Enum:futsal_position", "goalkeeper,defender,winger,forward")
                .Annotation("Npgsql:Enum:game_result", "white_win,black_win,draw,not_played")
                .Annotation("Npgsql:Enum:notification_type", "system,tournament,achievement,match,team")
                .Annotation("Npgsql:Enum:registration_status", "pending,approved,rejected")
                .Annotation("Npgsql:Enum:table_tennis_category", "men,women,mixed")
                .Annotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .Annotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .Annotation("Npgsql:Enum:user_role", "none,user,admin")
                .Annotation("Npgsql:Enum:volleyball_position", "setter,outside_hitter,middle_blocker,opposite,libero")
                .Annotation("Npgsql:Enum:volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff");

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.CheckConstraint("CK_RefreshToken_ExpiresAt", "expires_at >= now()");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    is_email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.CheckConstraint("CK_User_CreatedAt", "created_at <= now()");
                });

            migrationBuilder.CreateTable(
                name: "conversations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_message_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user1_id = table.Column<long>(type: "bigint", nullable: false),
                    user2_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversations", x => x.id);
                    table.ForeignKey(
                        name: "FK_conversations_users_user1_id",
                        column: x => x.user1_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_conversations_users_user2_id",
                        column: x => x.user2_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    link = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_notifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    Patronymic = table.Column<string>(type: "text", nullable: true),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    birthday = table.Column<DateOnly>(type: "date", nullable: false),
                    photo_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.id);
                    table.ForeignKey(
                        name: "FK_persons_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    conversation_id = table.Column<long>(type: "bigint", nullable: false),
                    sender_id = table.Column<long>(type: "bigint", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_messages_conversations_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "conversations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messages_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "checkers_referees",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkers_referees", x => x.id);
                    table.ForeignKey(
                        name: "FK_checkers_referees_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chess_referees",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chess_referees", x => x.id);
                    table.ForeignKey(
                        name: "FK_chess_referees_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "armwrestling_event_participants",
                columns: table => new
                {
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    Weight = table.Column<double>(type: "double precision", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_armwrestling_event_participants", x => new { x.TournamentId, x.PlayerId });
                });

            migrationBuilder.CreateTable(
                name: "armwrestling_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    hand = table.Column<int>(type: "integer", nullable: false),
                    weight_class = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    TimeControl = table.Column<string>(type: "text", nullable: true),
                    ParticipantsCount = table.Column<int>(type: "integer", nullable: false),
                    TournamentType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_armwrestling_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "armwrestling_matches",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_id = table.Column<long>(type: "bigint", nullable: false),
                    player1_id = table.Column<long>(type: "bigint", nullable: false),
                    player2_id = table.Column<long>(type: "bigint", nullable: false),
                    winner_id = table.Column<long>(type: "bigint", nullable: true),
                    stage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TableNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_armwrestling_matches", x => x.id);
                    table.ForeignKey(
                        name: "FK_armwrestling_matches_armwrestling_events_event_id",
                        column: x => x.event_id,
                        principalTable: "armwrestling_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "badminton_event_participants",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    Points = table.Column<double>(type: "double precision", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_event_participants", x => new { x.EventId, x.PlayerId });
                });

            migrationBuilder.CreateTable(
                name: "badminton_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    TournamentType = table.Column<int>(type: "integer", nullable: false),
                    FirstPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    SecondPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    ThirdPlaceTeamId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "badminton_match_away_roster",
                columns: table => new
                {
                    BadmintonMatchId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_match_away_roster", x => new { x.BadmintonMatchId, x.PlayerId });
                });

            migrationBuilder.CreateTable(
                name: "badminton_match_home_roster",
                columns: table => new
                {
                    BadmintonMatchId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_match_home_roster", x => new { x.BadmintonMatchId, x.PlayerId });
                });

            migrationBuilder.CreateTable(
                name: "badminton_matches",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    Player1Id = table.Column<long>(type: "bigint", nullable: true),
                    Player2Id = table.Column<long>(type: "bigint", nullable: true),
                    HomeTeamId = table.Column<long>(type: "bigint", nullable: true),
                    AwayTeamId = table.Column<long>(type: "bigint", nullable: true),
                    HomeScore = table.Column<int>(type: "integer", nullable: true),
                    AwayScore = table.Column<int>(type: "integer", nullable: true),
                    WinnerPlayerId = table.Column<long>(type: "bigint", nullable: true),
                    WinnerTeamId = table.Column<long>(type: "bigint", nullable: true),
                    Stage = table.Column<string>(type: "text", nullable: true),
                    CourtNumber = table.Column<int>(type: "integer", nullable: false),
                    SetsToWin = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_badminton_matches_badminton_events_EventId",
                        column: x => x.EventId,
                        principalTable: "badminton_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "badminton_sets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MatchId = table.Column<long>(type: "bigint", nullable: false),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    Player1Score = table.Column<int>(type: "integer", nullable: false),
                    Player2Score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_sets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_badminton_sets_badminton_matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "badminton_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "badminton_teams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Player1Id = table.Column<long>(type: "bigint", nullable: true),
                    Player2Id = table.Column<long>(type: "bigint", nullable: true),
                    EventId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_badminton_teams_badminton_events_EventId",
                        column: x => x.EventId,
                        principalTable: "badminton_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "basketball3x3_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    TournamentType = table.Column<int>(type: "integer", nullable: false),
                    FirstPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    SecondPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    ThirdPlaceTeamId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_basketball3x3_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "basketball3x3_teams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_basketball3x3_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_basketball3x3_teams_basketball3x3_events_EventId",
                        column: x => x.EventId,
                        principalTable: "basketball3x3_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "basketball3x3_matches",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    HomeTeamId = table.Column<long>(type: "bigint", nullable: false),
                    AwayTeamId = table.Column<long>(type: "bigint", nullable: false),
                    HomeTeamScore = table.Column<int>(type: "integer", nullable: true),
                    AwayTeamScore = table.Column<int>(type: "integer", nullable: true),
                    WinnerTeamId = table.Column<long>(type: "bigint", nullable: true),
                    Stage = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_basketball3x3_matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_basketball3x3_matches_basketball3x3_events_EventId",
                        column: x => x.EventId,
                        principalTable: "basketball3x3_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_basketball3x3_matches_basketball3x3_teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "basketball3x3_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_basketball3x3_matches_basketball3x3_teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "basketball3x3_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_basketball3x3_matches_basketball3x3_teams_WinnerTeamId",
                        column: x => x.WinnerTeamId,
                        principalTable: "basketball3x3_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "basketball3x3_match_away_roster",
                columns: table => new
                {
                    Basketball3x3MatchId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_basketball3x3_match_away_roster", x => new { x.Basketball3x3MatchId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_basketball3x3_match_away_roster_basketball3x3_matches_Baske~",
                        column: x => x.Basketball3x3MatchId,
                        principalTable: "basketball3x3_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "basketball3x3_match_home_roster",
                columns: table => new
                {
                    Basketball3x3MatchId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_basketball3x3_match_home_roster", x => new { x.Basketball3x3MatchId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_basketball3x3_match_home_roster_basketball3x3_matches_Baske~",
                        column: x => x.Basketball3x3MatchId,
                        principalTable: "basketball3x3_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beach_volleyball_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    TournamentType = table.Column<int>(type: "integer", nullable: false),
                    FirstPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    SecondPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    ThirdPlaceTeamId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beach_volleyball_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "beach_volleyball_teams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beach_volleyball_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_beach_volleyball_teams_beach_volleyball_events_EventId",
                        column: x => x.EventId,
                        principalTable: "beach_volleyball_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beach_volleyball_matches",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    HomeTeamId = table.Column<long>(type: "bigint", nullable: false),
                    AwayTeamId = table.Column<long>(type: "bigint", nullable: false),
                    SetsToWin = table.Column<int>(type: "integer", nullable: false),
                    HomeSetsWon = table.Column<int>(type: "integer", nullable: true),
                    AwaySetsWon = table.Column<int>(type: "integer", nullable: true),
                    WinnerTeamId = table.Column<long>(type: "bigint", nullable: true),
                    Stage = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beach_volleyball_matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_beach_volleyball_matches_beach_volleyball_events_EventId",
                        column: x => x.EventId,
                        principalTable: "beach_volleyball_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_beach_volleyball_matches_beach_volleyball_teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "beach_volleyball_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_beach_volleyball_matches_beach_volleyball_teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "beach_volleyball_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_beach_volleyball_matches_beach_volleyball_teams_WinnerTeamId",
                        column: x => x.WinnerTeamId,
                        principalTable: "beach_volleyball_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "beach_volleyball_sets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MatchId = table.Column<long>(type: "bigint", nullable: false),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    HomeScore = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    AwayScore = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beach_volleyball_sets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_beach_volleyball_sets_beach_volleyball_matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "beach_volleyball_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beach_volleyball_match_away_roster",
                columns: table => new
                {
                    BeachVolleyballMatchId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beach_volleyball_match_away_roster", x => new { x.BeachVolleyballMatchId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_beach_volleyball_match_away_roster_beach_volleyball_matches~",
                        column: x => x.BeachVolleyballMatchId,
                        principalTable: "beach_volleyball_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beach_volleyball_match_home_roster",
                columns: table => new
                {
                    BeachVolleyballMatchId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beach_volleyball_match_home_roster", x => new { x.BeachVolleyballMatchId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_beach_volleyball_match_home_roster_beach_volleyball_matches~",
                        column: x => x.BeachVolleyballMatchId,
                        principalTable: "beach_volleyball_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "checkers_event_participants",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    starting_rating = table.Column<int>(type: "integer", nullable: false),
                    points = table.Column<double>(type: "double precision", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkers_event_participants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "checkers_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    time_control = table.Column<string>(type: "text", nullable: false),
                    participants_count = table.Column<int>(type: "integer", nullable: false),
                    system = table.Column<int>(type: "integer", nullable: false),
                    variant = table.Column<int>(type: "integer", nullable: false),
                    FirstPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    SecondPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    ThirdPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkers_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "checkers_teams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkers_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_checkers_teams_checkers_events_EventId",
                        column: x => x.EventId,
                        principalTable: "checkers_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "checkers_games",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    checkers_event_id = table.Column<long>(type: "bigint", nullable: false),
                    round_number = table.Column<int>(type: "integer", nullable: false),
                    white_player_id = table.Column<long>(type: "bigint", nullable: true),
                    black_player_id = table.Column<long>(type: "bigint", nullable: true),
                    white_team_id = table.Column<long>(type: "bigint", nullable: true),
                    black_team_id = table.Column<long>(type: "bigint", nullable: true),
                    white_team_score = table.Column<double>(type: "double precision", nullable: true),
                    black_team_score = table.Column<double>(type: "double precision", nullable: true),
                    result = table.Column<int>(type: "integer", nullable: false),
                    scheduled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    finished_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkers_games", x => x.id);
                    table.ForeignKey(
                        name: "FK_checkers_games_checkers_event_participants_black_player_id",
                        column: x => x.black_player_id,
                        principalTable: "checkers_event_participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_checkers_games_checkers_event_participants_white_player_id",
                        column: x => x.white_player_id,
                        principalTable: "checkers_event_participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_checkers_games_checkers_events_checkers_event_id",
                        column: x => x.checkers_event_id,
                        principalTable: "checkers_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_checkers_games_checkers_teams_black_team_id",
                        column: x => x.black_team_id,
                        principalTable: "checkers_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_checkers_games_checkers_teams_white_team_id",
                        column: x => x.white_team_id,
                        principalTable: "checkers_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chess_event_participants",
                columns: table => new
                {
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    points = table.Column<double>(type: "double precision", nullable: false),
                    rank = table.Column<int>(type: "integer", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chess_event_participants", x => new { x.TournamentId, x.PlayerId });
                });

            migrationBuilder.CreateTable(
                name: "chess_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    time_control = table.Column<string>(type: "text", nullable: false),
                    participants_count = table.Column<int>(type: "integer", nullable: false),
                    system = table.Column<int>(type: "integer", nullable: false),
                    FirstPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    SecondPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    ThirdPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chess_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chess_teams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chess_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chess_teams_chess_events_EventId",
                        column: x => x.EventId,
                        principalTable: "chess_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chess_games",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tournament_id = table.Column<long>(type: "bigint", nullable: false),
                    white_player_id = table.Column<long>(type: "bigint", nullable: true),
                    black_player_id = table.Column<long>(type: "bigint", nullable: true),
                    white_team_id = table.Column<long>(type: "bigint", nullable: true),
                    black_team_id = table.Column<long>(type: "bigint", nullable: true),
                    white_team_score = table.Column<double>(type: "double precision", nullable: true),
                    black_team_score = table.Column<double>(type: "double precision", nullable: true),
                    result = table.Column<int>(type: "integer", nullable: false),
                    round = table.Column<int>(type: "integer", nullable: false),
                    board = table.Column<int>(type: "integer", nullable: false),
                    pgn = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chess_games", x => x.id);
                    table.ForeignKey(
                        name: "FK_chess_games_chess_events_tournament_id",
                        column: x => x.tournament_id,
                        principalTable: "chess_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_chess_games_chess_teams_black_team_id",
                        column: x => x.black_team_id,
                        principalTable: "chess_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chess_games_chess_teams_white_team_id",
                        column: x => x.white_team_id,
                        principalTable: "chess_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "event_photos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    url = table.Column<string>(type: "text", nullable: false),
                    event_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_photos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    location = table.Column<string>(type: "text", nullable: false),
                    referee_ids = table.Column<long[]>(type: "bigint[]", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    registration_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    registration_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Rules = table.Column<string>(type: "text", nullable: true),
                    Prizes = table.Column<string>(type: "text", nullable: true),
                    HeadRefereeName = table.Column<string>(type: "text", nullable: true),
                    Referees = table.Column<string>(type: "text", nullable: true),
                    Schedule = table.Column<string>(type: "text", nullable: true),
                    IsTeam = table.Column<bool>(type: "boolean", nullable: false),
                    IsModerated = table.Column<bool>(type: "boolean", nullable: false),
                    FirstPlaceId = table.Column<long>(type: "bigint", nullable: true),
                    SecondPlaceId = table.Column<long>(type: "bigint", nullable: true),
                    ThirdPlaceId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "table_tennis_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    tournament_type = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_tennis_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_tennis_events_events_id",
                        column: x => x.id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "futsal_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    TournamentType = table.Column<int>(type: "integer", nullable: false),
                    FirstPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    SecondPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    ThirdPlaceTeamId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_futsal_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_futsal_events_events_id",
                        column: x => x.id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "futsal_teams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_futsal_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_futsal_teams_futsal_events_EventId",
                        column: x => x.EventId,
                        principalTable: "futsal_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "futsal_matches",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    HomeTeamId = table.Column<long>(type: "bigint", nullable: false),
                    AwayTeamId = table.Column<long>(type: "bigint", nullable: false),
                    HomeTeamScore = table.Column<int>(type: "integer", nullable: true),
                    AwayTeamScore = table.Column<int>(type: "integer", nullable: true),
                    WinnerTeamId = table.Column<long>(type: "bigint", nullable: true),
                    Stage = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_futsal_matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_futsal_matches_futsal_events_EventId",
                        column: x => x.EventId,
                        principalTable: "futsal_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_futsal_matches_futsal_teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "futsal_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_futsal_matches_futsal_teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "futsal_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_futsal_matches_futsal_teams_WinnerTeamId",
                        column: x => x.WinnerTeamId,
                        principalTable: "futsal_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "futsal_goals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MatchId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    GoalsCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_futsal_goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_futsal_goals_futsal_matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "futsal_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "futsal_match_away_roster",
                columns: table => new
                {
                    FutsalMatchId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_futsal_match_away_roster", x => new { x.FutsalMatchId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_futsal_match_away_roster_futsal_matches_FutsalMatchId",
                        column: x => x.FutsalMatchId,
                        principalTable: "futsal_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "futsal_match_home_roster",
                columns: table => new
                {
                    FutsalMatchId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_futsal_match_home_roster", x => new { x.FutsalMatchId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_futsal_match_home_roster_futsal_matches_FutsalMatchId",
                        column: x => x.FutsalMatchId,
                        principalTable: "futsal_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    group = table.Column<string>(type: "text", nullable: false),
                    EnrollmentYear = table.Column<int>(type: "integer", nullable: false),
                    SpecialtyCode = table.Column<int>(type: "integer", nullable: false),
                    GroupNumber = table.Column<int>(type: "integer", nullable: false),
                    student_id = table.Column<string>(type: "text", nullable: true),
                    Faculty = table.Column<string>(type: "text", nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    InstagramUrl = table.Column<string>(type: "text", nullable: true),
                    chess_rating = table.Column<int>(type: "integer", nullable: true),
                    chess_fide_id = table.Column<string>(type: "text", nullable: true),
                    chess_title = table.Column<string>(type: "text", nullable: true),
                    checkers_rating = table.Column<int>(type: "integer", nullable: true),
                    table_tennis_rating = table.Column<int>(type: "integer", nullable: true),
                    table_tennis_hand = table.Column<int>(type: "integer", nullable: true),
                    armwrestling_weight = table.Column<float>(type: "real", nullable: true),
                    armwrestling_preferred_weight_class = table.Column<int>(type: "integer", nullable: true),
                    futsal_position = table.Column<int>(type: "integer", nullable: true),
                    futsal_shirt_number = table.Column<int>(type: "integer", nullable: true),
                    futsal_team_id = table.Column<long>(type: "bigint", nullable: true),
                    volleyball_position = table.Column<int>(type: "integer", nullable: true),
                    volleyball_shirt_number = table.Column<int>(type: "integer", nullable: true),
                    volleyball_team_id = table.Column<long>(type: "bigint", nullable: true),
                    basketball_shirt_number = table.Column<int>(type: "integer", nullable: true),
                    basketball_team_id = table.Column<long>(type: "bigint", nullable: true),
                    beach_volleyball_shirt_number = table.Column<int>(type: "integer", nullable: true),
                    beach_volleyball_team_id = table.Column<long>(type: "bigint", nullable: true),
                    Basketball3x3TeamId = table.Column<long>(type: "bigint", nullable: true),
                    CheckersTeamId = table.Column<long>(type: "bigint", nullable: true),
                    ChessTeamId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_players_basketball3x3_teams_Basketball3x3TeamId",
                        column: x => x.Basketball3x3TeamId,
                        principalTable: "basketball3x3_teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_players_beach_volleyball_teams_beach_volleyball_team_id",
                        column: x => x.beach_volleyball_team_id,
                        principalTable: "beach_volleyball_teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_players_checkers_teams_CheckersTeamId",
                        column: x => x.CheckersTeamId,
                        principalTable: "checkers_teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_players_chess_teams_ChessTeamId",
                        column: x => x.ChessTeamId,
                        principalTable: "chess_teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_players_futsal_teams_futsal_team_id",
                        column: x => x.futsal_team_id,
                        principalTable: "futsal_teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_players_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "table_tennis_event_participants",
                columns: table => new
                {
                    tournament_id = table.Column<long>(type: "bigint", nullable: false),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    points = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    rank = table.Column<int>(type: "integer", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_tennis_event_participants", x => new { x.tournament_id, x.player_id });
                    table.ForeignKey(
                        name: "FK_table_tennis_event_participants_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_table_tennis_event_participants_table_tennis_events_tournam~",
                        column: x => x.tournament_id,
                        principalTable: "table_tennis_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "table_tennis_matches",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tournament_id = table.Column<long>(type: "bigint", nullable: false),
                    player1_id = table.Column<long>(type: "bigint", nullable: false),
                    player2_id = table.Column<long>(type: "bigint", nullable: false),
                    winner_id = table.Column<long>(type: "bigint", nullable: true),
                    stage = table.Column<string>(type: "text", nullable: true),
                    table_number = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    sets_to_win = table.Column<int>(type: "integer", nullable: false, defaultValue: 3)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_tennis_matches", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_tennis_matches_players_player1_id",
                        column: x => x.player1_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_table_tennis_matches_players_player2_id",
                        column: x => x.player2_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_table_tennis_matches_players_winner_id",
                        column: x => x.winner_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_table_tennis_matches_table_tennis_events_tournament_id",
                        column: x => x.tournament_id,
                        principalTable: "table_tennis_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "table_tennis_sets",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    set_number = table.Column<int>(type: "integer", nullable: false),
                    player1_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    player2_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_tennis_sets", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_tennis_sets_table_tennis_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "table_tennis_matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "volleyball_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    TournamentType = table.Column<int>(type: "integer", nullable: false),
                    FirstPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    SecondPlaceTeamId = table.Column<long>(type: "bigint", nullable: true),
                    ThirdPlaceTeamId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volleyball_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_volleyball_events_events_id",
                        column: x => x.id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "volleyball_teams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volleyball_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_volleyball_teams_volleyball_events_EventId",
                        column: x => x.EventId,
                        principalTable: "volleyball_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "volleyball_matches",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    HomeTeamId = table.Column<long>(type: "bigint", nullable: false),
                    AwayTeamId = table.Column<long>(type: "bigint", nullable: false),
                    SetsToWin = table.Column<int>(type: "integer", nullable: false),
                    HomeSetsWon = table.Column<int>(type: "integer", nullable: true),
                    AwaySetsWon = table.Column<int>(type: "integer", nullable: true),
                    WinnerTeamId = table.Column<long>(type: "bigint", nullable: true),
                    Stage = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volleyball_matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_volleyball_matches_volleyball_events_EventId",
                        column: x => x.EventId,
                        principalTable: "volleyball_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_volleyball_matches_volleyball_teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "volleyball_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_volleyball_matches_volleyball_teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "volleyball_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_volleyball_matches_volleyball_teams_WinnerTeamId",
                        column: x => x.WinnerTeamId,
                        principalTable: "volleyball_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "volleyball_match_away_roster",
                columns: table => new
                {
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    VolleyballMatchId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volleyball_match_away_roster", x => new { x.PlayerId, x.VolleyballMatchId });
                    table.ForeignKey(
                        name: "FK_volleyball_match_away_roster_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_volleyball_match_away_roster_volleyball_matches_VolleyballM~",
                        column: x => x.VolleyballMatchId,
                        principalTable: "volleyball_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "volleyball_match_home_roster",
                columns: table => new
                {
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    VolleyballMatchId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volleyball_match_home_roster", x => new { x.PlayerId, x.VolleyballMatchId });
                    table.ForeignKey(
                        name: "FK_volleyball_match_home_roster_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_volleyball_match_home_roster_volleyball_matches_VolleyballM~",
                        column: x => x.VolleyballMatchId,
                        principalTable: "volleyball_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "volleyball_sets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MatchId = table.Column<long>(type: "bigint", nullable: false),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    HomeScore = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    AwayScore = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volleyball_sets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_volleyball_sets_volleyball_matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "volleyball_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_armwrestling_event_participants_PlayerId",
                table: "armwrestling_event_participants",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_arm_events_category",
                table: "armwrestling_events",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_arm_events_hand",
                table: "armwrestling_events",
                column: "hand");

            migrationBuilder.CreateIndex(
                name: "IX_arm_events_weight_class",
                table: "armwrestling_events",
                column: "weight_class");

            migrationBuilder.CreateIndex(
                name: "IX_arm_matches_event_id",
                table: "armwrestling_matches",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_arm_matches_player1_id",
                table: "armwrestling_matches",
                column: "player1_id");

            migrationBuilder.CreateIndex(
                name: "IX_arm_matches_player2_id",
                table: "armwrestling_matches",
                column: "player2_id");

            migrationBuilder.CreateIndex(
                name: "IX_armwrestling_matches_winner_id",
                table: "armwrestling_matches",
                column: "winner_id");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_event_participants_PlayerId",
                table: "badminton_event_participants",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_events_FirstPlaceTeamId",
                table: "badminton_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_events_SecondPlaceTeamId",
                table: "badminton_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_events_ThirdPlaceTeamId",
                table: "badminton_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_match_away_roster_PlayerId",
                table: "badminton_match_away_roster",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_match_home_roster_PlayerId",
                table: "badminton_match_home_roster",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_AwayTeamId",
                table: "badminton_matches",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_EventId",
                table: "badminton_matches",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_HomeTeamId",
                table: "badminton_matches",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_Player1Id",
                table: "badminton_matches",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_Player2Id",
                table: "badminton_matches",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_WinnerPlayerId",
                table: "badminton_matches",
                column: "WinnerPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_WinnerTeamId",
                table: "badminton_matches",
                column: "WinnerTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_sets_MatchId",
                table: "badminton_sets",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_teams_EventId",
                table: "badminton_teams",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_teams_Player1Id",
                table: "badminton_teams",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_teams_Player2Id",
                table: "badminton_teams",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_events_FirstPlaceTeamId",
                table: "basketball3x3_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_events_SecondPlaceTeamId",
                table: "basketball3x3_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_events_ThirdPlaceTeamId",
                table: "basketball3x3_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_match_away_roster_PlayerId",
                table: "basketball3x3_match_away_roster",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_match_home_roster_PlayerId",
                table: "basketball3x3_match_home_roster",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_matches_AwayTeamId",
                table: "basketball3x3_matches",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_matches_EventId",
                table: "basketball3x3_matches",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_matches_HomeTeamId",
                table: "basketball3x3_matches",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_matches_WinnerTeamId",
                table: "basketball3x3_matches",
                column: "WinnerTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_teams_EventId",
                table: "basketball3x3_teams",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_events_FirstPlaceTeamId",
                table: "beach_volleyball_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_events_SecondPlaceTeamId",
                table: "beach_volleyball_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_events_ThirdPlaceTeamId",
                table: "beach_volleyball_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_match_away_roster_PlayerId",
                table: "beach_volleyball_match_away_roster",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_match_home_roster_PlayerId",
                table: "beach_volleyball_match_home_roster",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_matches_AwayTeamId",
                table: "beach_volleyball_matches",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_matches_EventId",
                table: "beach_volleyball_matches",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_matches_HomeTeamId",
                table: "beach_volleyball_matches",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_matches_WinnerTeamId",
                table: "beach_volleyball_matches",
                column: "WinnerTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_sets_match_setnumber",
                table: "beach_volleyball_sets",
                columns: new[] { "MatchId", "SetNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_teams_EventId",
                table: "beach_volleyball_teams",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_event_participants_event_id",
                table: "checkers_event_participants",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_event_participants_event_points",
                table: "checkers_event_participants",
                columns: new[] { "TournamentId", "points" });

            migrationBuilder.CreateIndex(
                name: "IX_checkers_event_participants_event_user",
                table: "checkers_event_participants",
                columns: new[] { "TournamentId", "PlayerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_checkers_event_participants_user_id",
                table: "checkers_event_participants",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_events_FirstPlaceTeamId",
                table: "checkers_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_events_SecondPlaceTeamId",
                table: "checkers_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_events_system",
                table: "checkers_events",
                column: "system");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_events_ThirdPlaceTeamId",
                table: "checkers_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_events_variant",
                table: "checkers_events",
                column: "variant");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_black_player_id",
                table: "checkers_games",
                column: "black_player_id");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_black_team_id",
                table: "checkers_games",
                column: "black_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_event_black",
                table: "checkers_games",
                columns: new[] { "checkers_event_id", "black_player_id" });

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_event_id",
                table: "checkers_games",
                column: "checkers_event_id");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_event_round",
                table: "checkers_games",
                columns: new[] { "checkers_event_id", "round_number" });

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_event_white",
                table: "checkers_games",
                columns: new[] { "checkers_event_id", "white_player_id" });

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_result",
                table: "checkers_games",
                column: "result");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_white_player_id",
                table: "checkers_games",
                column: "white_player_id");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_white_team_id",
                table: "checkers_games",
                column: "white_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_teams_EventId",
                table: "checkers_teams",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_event_participants_player_id",
                table: "chess_event_participants",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_event_participants_tournament_id",
                table: "chess_event_participants",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_event_participants_tournament_points_rank",
                table: "chess_event_participants",
                columns: new[] { "TournamentId", "points", "rank" });

            migrationBuilder.CreateIndex(
                name: "IX_chess_events_FirstPlaceTeamId",
                table: "chess_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_events_SecondPlaceTeamId",
                table: "chess_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_events_system",
                table: "chess_events",
                column: "system");

            migrationBuilder.CreateIndex(
                name: "IX_chess_events_ThirdPlaceTeamId",
                table: "chess_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_black_player_id",
                table: "chess_games",
                column: "black_player_id");

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_black_team_id",
                table: "chess_games",
                column: "black_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_result",
                table: "chess_games",
                column: "result");

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_tournament_black",
                table: "chess_games",
                columns: new[] { "tournament_id", "black_player_id" });

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_tournament_id",
                table: "chess_games",
                column: "tournament_id");

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_tournament_round",
                table: "chess_games",
                columns: new[] { "tournament_id", "round" });

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_tournament_white",
                table: "chess_games",
                columns: new[] { "tournament_id", "white_player_id" });

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_white_player_id",
                table: "chess_games",
                column: "white_player_id");

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_white_team_id",
                table: "chess_games",
                column: "white_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_chess_teams_EventId",
                table: "chess_teams",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_conversations_user1_id_user2_id",
                table: "conversations",
                columns: new[] { "user1_id", "user2_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_conversations_user2_id",
                table: "conversations",
                column: "user2_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_photos_event_id",
                table: "event_photos",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_events_date",
                table: "events",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_events_FirstPlaceId",
                table: "events",
                column: "FirstPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_events_level",
                table: "events",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "IX_events_SecondPlaceId",
                table: "events",
                column: "SecondPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_events_ThirdPlaceId",
                table: "events",
                column: "ThirdPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_events_FirstPlaceTeamId",
                table: "futsal_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_events_SecondPlaceTeamId",
                table: "futsal_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_events_ThirdPlaceTeamId",
                table: "futsal_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_goals_MatchId",
                table: "futsal_goals",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_goals_PlayerId",
                table: "futsal_goals",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_match_away_roster_PlayerId",
                table: "futsal_match_away_roster",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_match_home_roster_PlayerId",
                table: "futsal_match_home_roster",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_matches_AwayTeamId",
                table: "futsal_matches",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_matches_EventId",
                table: "futsal_matches",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_matches_HomeTeamId",
                table: "futsal_matches",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_matches_WinnerTeamId",
                table: "futsal_matches",
                column: "WinnerTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_teams_EventId",
                table: "futsal_teams",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_conversation_id",
                table: "messages",
                column: "conversation_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_sender_id",
                table: "messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_user_id",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_persons_user_id",
                table: "persons",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_players_Basketball3x3TeamId",
                table: "players",
                column: "Basketball3x3TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_players_beach_volleyball_team_id",
                table: "players",
                column: "beach_volleyball_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_players_CheckersTeamId",
                table: "players",
                column: "CheckersTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_players_ChessTeamId",
                table: "players",
                column: "ChessTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_players_futsal_team_id",
                table: "players",
                column: "futsal_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_players_student_id",
                table: "players",
                column: "student_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_players_volleyball_team_id",
                table: "players",
                column: "volleyball_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id_active",
                table: "refresh_tokens",
                columns: new[] { "user_id", "is_revoked", "expires_at" });

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_event_participants_player_id",
                table: "table_tennis_event_participants",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_tt_events_category",
                table: "table_tennis_events",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_tt_events_type",
                table: "table_tennis_events",
                column: "tournament_type");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_matches_player1_id",
                table: "table_tennis_matches",
                column: "player1_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_matches_player2_id",
                table: "table_tennis_matches",
                column: "player2_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_matches_winner_id",
                table: "table_tennis_matches",
                column: "winner_id");

            migrationBuilder.CreateIndex(
                name: "IX_tt_matches_tournament_id",
                table: "table_tennis_matches",
                column: "tournament_id");

            migrationBuilder.CreateIndex(
                name: "IX_tt_sets_match_setnumber",
                table: "table_tennis_sets",
                columns: new[] { "match_id", "set_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_events_FirstPlaceTeamId",
                table: "volleyball_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_events_SecondPlaceTeamId",
                table: "volleyball_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_events_ThirdPlaceTeamId",
                table: "volleyball_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_match_away_roster_VolleyballMatchId",
                table: "volleyball_match_away_roster",
                column: "VolleyballMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_match_home_roster_VolleyballMatchId",
                table: "volleyball_match_home_roster",
                column: "VolleyballMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_matches_AwayTeamId",
                table: "volleyball_matches",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_matches_EventId",
                table: "volleyball_matches",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_matches_HomeTeamId",
                table: "volleyball_matches",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_matches_WinnerTeamId",
                table: "volleyball_matches",
                column: "WinnerTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_sets_match_setnumber",
                table: "volleyball_sets",
                columns: new[] { "MatchId", "SetNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_teams_EventId",
                table: "volleyball_teams",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_event_participants_armwrestling_events_Tournam~",
                table: "armwrestling_event_participants",
                column: "TournamentId",
                principalTable: "armwrestling_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_event_participants_players_PlayerId",
                table: "armwrestling_event_participants",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_events_events_id",
                table: "armwrestling_events",
                column: "id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_matches_players_player1_id",
                table: "armwrestling_matches",
                column: "player1_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_matches_players_player2_id",
                table: "armwrestling_matches",
                column: "player2_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_matches_players_winner_id",
                table: "armwrestling_matches",
                column: "winner_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_event_participants_badminton_events_EventId",
                table: "badminton_event_participants",
                column: "EventId",
                principalTable: "badminton_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_event_participants_players_PlayerId",
                table: "badminton_event_participants",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_events_badminton_teams_FirstPlaceTeamId",
                table: "badminton_events",
                column: "FirstPlaceTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_events_badminton_teams_SecondPlaceTeamId",
                table: "badminton_events",
                column: "SecondPlaceTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_events_badminton_teams_ThirdPlaceTeamId",
                table: "badminton_events",
                column: "ThirdPlaceTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_events_events_id",
                table: "badminton_events",
                column: "id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_match_away_roster_badminton_matches_BadmintonMatc~",
                table: "badminton_match_away_roster",
                column: "BadmintonMatchId",
                principalTable: "badminton_matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_match_away_roster_players_PlayerId",
                table: "badminton_match_away_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_match_home_roster_badminton_matches_BadmintonMatc~",
                table: "badminton_match_home_roster",
                column: "BadmintonMatchId",
                principalTable: "badminton_matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_match_home_roster_players_PlayerId",
                table: "badminton_match_home_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_teams_AwayTeamId",
                table: "badminton_matches",
                column: "AwayTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_teams_HomeTeamId",
                table: "badminton_matches",
                column: "HomeTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_teams_WinnerTeamId",
                table: "badminton_matches",
                column: "WinnerTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_players_Player1Id",
                table: "badminton_matches",
                column: "Player1Id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_players_Player2Id",
                table: "badminton_matches",
                column: "Player2Id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_players_WinnerPlayerId",
                table: "badminton_matches",
                column: "WinnerPlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_teams_players_Player1Id",
                table: "badminton_teams",
                column: "Player1Id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_teams_players_Player2Id",
                table: "badminton_teams",
                column: "Player2Id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_FirstPlaceTeamId",
                table: "basketball3x3_events",
                column: "FirstPlaceTeamId",
                principalTable: "basketball3x3_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_SecondPlaceTeamId",
                table: "basketball3x3_events",
                column: "SecondPlaceTeamId",
                principalTable: "basketball3x3_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_ThirdPlaceTeamId",
                table: "basketball3x3_events",
                column: "ThirdPlaceTeamId",
                principalTable: "basketball3x3_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_events_events_id",
                table: "basketball3x3_events",
                column: "id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_match_away_roster_players_PlayerId",
                table: "basketball3x3_match_away_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_match_home_roster_players_PlayerId",
                table: "basketball3x3_match_home_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_FirstPlaceTe~",
                table: "beach_volleyball_events",
                column: "FirstPlaceTeamId",
                principalTable: "beach_volleyball_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_SecondPlaceT~",
                table: "beach_volleyball_events",
                column: "SecondPlaceTeamId",
                principalTable: "beach_volleyball_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_ThirdPlaceTe~",
                table: "beach_volleyball_events",
                column: "ThirdPlaceTeamId",
                principalTable: "beach_volleyball_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_events_events_id",
                table: "beach_volleyball_events",
                column: "id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_match_away_roster_players_PlayerId",
                table: "beach_volleyball_match_away_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_match_home_roster_players_PlayerId",
                table: "beach_volleyball_match_home_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_event_participants_checkers_events_TournamentId",
                table: "checkers_event_participants",
                column: "TournamentId",
                principalTable: "checkers_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_event_participants_players_PlayerId",
                table: "checkers_event_participants",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_events_checkers_teams_FirstPlaceTeamId",
                table: "checkers_events",
                column: "FirstPlaceTeamId",
                principalTable: "checkers_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_events_checkers_teams_SecondPlaceTeamId",
                table: "checkers_events",
                column: "SecondPlaceTeamId",
                principalTable: "checkers_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_events_checkers_teams_ThirdPlaceTeamId",
                table: "checkers_events",
                column: "ThirdPlaceTeamId",
                principalTable: "checkers_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_events_events_id",
                table: "checkers_events",
                column: "id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_event_participants_chess_events_TournamentId",
                table: "chess_event_participants",
                column: "TournamentId",
                principalTable: "chess_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_event_participants_players_PlayerId",
                table: "chess_event_participants",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_events_chess_teams_FirstPlaceTeamId",
                table: "chess_events",
                column: "FirstPlaceTeamId",
                principalTable: "chess_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_events_chess_teams_SecondPlaceTeamId",
                table: "chess_events",
                column: "SecondPlaceTeamId",
                principalTable: "chess_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_events_chess_teams_ThirdPlaceTeamId",
                table: "chess_events",
                column: "ThirdPlaceTeamId",
                principalTable: "chess_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_events_events_id",
                table: "chess_events",
                column: "id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_players_black_player_id",
                table: "chess_games",
                column: "black_player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_players_white_player_id",
                table: "chess_games",
                column: "white_player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_event_photos_events_event_id",
                table: "event_photos",
                column: "event_id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_events_players_FirstPlaceId",
                table: "events",
                column: "FirstPlaceId",
                principalTable: "players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_events_players_SecondPlaceId",
                table: "events",
                column: "SecondPlaceId",
                principalTable: "players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_events_players_ThirdPlaceId",
                table: "events",
                column: "ThirdPlaceId",
                principalTable: "players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_events_futsal_teams_FirstPlaceTeamId",
                table: "futsal_events",
                column: "FirstPlaceTeamId",
                principalTable: "futsal_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_events_futsal_teams_SecondPlaceTeamId",
                table: "futsal_events",
                column: "SecondPlaceTeamId",
                principalTable: "futsal_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_events_futsal_teams_ThirdPlaceTeamId",
                table: "futsal_events",
                column: "ThirdPlaceTeamId",
                principalTable: "futsal_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_goals_players_PlayerId",
                table: "futsal_goals",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_match_away_roster_players_PlayerId",
                table: "futsal_match_away_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_match_home_roster_players_PlayerId",
                table: "futsal_match_home_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_players_volleyball_teams_volleyball_team_id",
                table: "players",
                column: "volleyball_team_id",
                principalTable: "volleyball_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_events_volleyball_teams_FirstPlaceTeamId",
                table: "volleyball_events",
                column: "FirstPlaceTeamId",
                principalTable: "volleyball_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_events_volleyball_teams_SecondPlaceTeamId",
                table: "volleyball_events",
                column: "SecondPlaceTeamId",
                principalTable: "volleyball_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_events_volleyball_teams_ThirdPlaceTeamId",
                table: "volleyball_events",
                column: "ThirdPlaceTeamId",
                principalTable: "volleyball_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_badminton_teams_players_Player1Id",
                table: "badminton_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_teams_players_Player2Id",
                table: "badminton_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_events_players_FirstPlaceId",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "FK_events_players_SecondPlaceId",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "FK_events_players_ThirdPlaceId",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_events_events_id",
                table: "badminton_events");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_events_events_id",
                table: "basketball3x3_events");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_events_events_id",
                table: "beach_volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_checkers_events_events_id",
                table: "checkers_events");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_events_events_id",
                table: "chess_events");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_events_events_id",
                table: "futsal_events");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_events_events_id",
                table: "volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_teams_badminton_events_EventId",
                table: "badminton_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_FirstPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_SecondPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_ThirdPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_FirstPlaceTe~",
                table: "beach_volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_SecondPlaceT~",
                table: "beach_volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_ThirdPlaceTe~",
                table: "beach_volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_checkers_teams_checkers_events_EventId",
                table: "checkers_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_teams_chess_events_EventId",
                table: "chess_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_events_futsal_teams_FirstPlaceTeamId",
                table: "futsal_events");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_events_futsal_teams_SecondPlaceTeamId",
                table: "futsal_events");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_events_futsal_teams_ThirdPlaceTeamId",
                table: "futsal_events");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_events_volleyball_teams_FirstPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_events_volleyball_teams_SecondPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_events_volleyball_teams_ThirdPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropTable(
                name: "armwrestling_event_participants");

            migrationBuilder.DropTable(
                name: "armwrestling_matches");

            migrationBuilder.DropTable(
                name: "badminton_event_participants");

            migrationBuilder.DropTable(
                name: "badminton_match_away_roster");

            migrationBuilder.DropTable(
                name: "badminton_match_home_roster");

            migrationBuilder.DropTable(
                name: "badminton_sets");

            migrationBuilder.DropTable(
                name: "basketball3x3_match_away_roster");

            migrationBuilder.DropTable(
                name: "basketball3x3_match_home_roster");

            migrationBuilder.DropTable(
                name: "beach_volleyball_match_away_roster");

            migrationBuilder.DropTable(
                name: "beach_volleyball_match_home_roster");

            migrationBuilder.DropTable(
                name: "beach_volleyball_sets");

            migrationBuilder.DropTable(
                name: "checkers_games");

            migrationBuilder.DropTable(
                name: "checkers_referees");

            migrationBuilder.DropTable(
                name: "chess_event_participants");

            migrationBuilder.DropTable(
                name: "chess_games");

            migrationBuilder.DropTable(
                name: "chess_referees");

            migrationBuilder.DropTable(
                name: "event_photos");

            migrationBuilder.DropTable(
                name: "futsal_goals");

            migrationBuilder.DropTable(
                name: "futsal_match_away_roster");

            migrationBuilder.DropTable(
                name: "futsal_match_home_roster");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "table_tennis_event_participants");

            migrationBuilder.DropTable(
                name: "table_tennis_sets");

            migrationBuilder.DropTable(
                name: "volleyball_match_away_roster");

            migrationBuilder.DropTable(
                name: "volleyball_match_home_roster");

            migrationBuilder.DropTable(
                name: "volleyball_sets");

            migrationBuilder.DropTable(
                name: "armwrestling_events");

            migrationBuilder.DropTable(
                name: "badminton_matches");

            migrationBuilder.DropTable(
                name: "basketball3x3_matches");

            migrationBuilder.DropTable(
                name: "beach_volleyball_matches");

            migrationBuilder.DropTable(
                name: "checkers_event_participants");

            migrationBuilder.DropTable(
                name: "futsal_matches");

            migrationBuilder.DropTable(
                name: "conversations");

            migrationBuilder.DropTable(
                name: "table_tennis_matches");

            migrationBuilder.DropTable(
                name: "volleyball_matches");

            migrationBuilder.DropTable(
                name: "table_tennis_events");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "persons");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "badminton_events");

            migrationBuilder.DropTable(
                name: "badminton_teams");

            migrationBuilder.DropTable(
                name: "basketball3x3_teams");

            migrationBuilder.DropTable(
                name: "basketball3x3_events");

            migrationBuilder.DropTable(
                name: "beach_volleyball_teams");

            migrationBuilder.DropTable(
                name: "beach_volleyball_events");

            migrationBuilder.DropTable(
                name: "checkers_events");

            migrationBuilder.DropTable(
                name: "checkers_teams");

            migrationBuilder.DropTable(
                name: "chess_events");

            migrationBuilder.DropTable(
                name: "chess_teams");

            migrationBuilder.DropTable(
                name: "futsal_teams");

            migrationBuilder.DropTable(
                name: "futsal_events");

            migrationBuilder.DropTable(
                name: "volleyball_teams");

            migrationBuilder.DropTable(
                name: "volleyball_events");
        }
    }
}
