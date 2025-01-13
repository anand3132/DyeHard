using System;
using System.Collections.Generic;
using PaintCore;
using UnityEngine;

namespace RedGaint
{

    public class TeamData
    {
        public Color TeamColor { get; private set; }
        public CwColor TeamColorComponent { get; private set; }
        public string TeamName { get; private set; }
        public HashSet<string> MemberNames { get; private set; }
        public uint TeamStrength { get; set; }
        public TeamData(Color teamColor, string teamName,CwColor teamColorComponent)
        {
            TeamColor = teamColor;
            TeamName = teamName;
            MemberNames = new HashSet<string>();
            TeamColorComponent = teamColorComponent;
            TeamStrength = 0;
        }
        public bool AddMember(string memberName)
        {
            return MemberNames.Add(memberName); // Add returns false if the member already exists
        }
        public bool RemoveMember(string memberName)
        {
            return MemberNames.Remove(memberName); // Remove returns false if the member does not exist
        }
    }

    public class TeamManager : Singleton<TeamManager>, IBugsBunny
    {
        [SerializeField] private  Dictionary<GlobalEnums.GameTeam, TeamData> teams = new Dictionary<GlobalEnums.GameTeam, TeamData>();
        private  HashSet<string> globalMembers = new HashSet<string>();
        public  bool RegisterTeam(GlobalEnums.GameTeam teamId, Color teamColor, string teamName)
        {
            if (teamId == GlobalEnums.GameTeam.None || teams.ContainsKey(teamId))
            {
                BugsBunny.Log($"Team {teamId} is already registered or invalid.",this);
                return false;
            }

            // Create team GameObject
            var teamGameObject = new GameObject(teamId.ToString());
            // Attach CwColor and set color
            var cwColor = teamGameObject.AddComponent<CwColor>();
            cwColor.Color = teamColor;
            teamGameObject.transform.SetParent(transform);

            // Register team
            teams[teamId] = new TeamData(teamColor, teamName, cwColor);
            BugsBunny.Log($"Team {teamName} registered successfully.",this);
            return true;
        }
        private GameObject parentObject;
        public  bool UnregisterTeam(GlobalEnums.GameTeam teamId)
        {
            if (teams.ContainsKey(teamId))
            {
                var team = teams[teamId];

                // Remove all members of the team from the global member list
                foreach (var member in team.MemberNames)
                {
                    globalMembers.Remove(member);
                }

                teams.Remove(teamId);
                BugsBunny.Log($"Team {teamId} unregistered successfully.",this);
                return true;
            }

            BugsBunny.Log($"Team {teamId} is not registered.",this);
            return false;
        }
        public  bool AddMemberToTeam(GlobalEnums.GameTeam teamId, string memberName)
        {
            if (teams.TryGetValue(teamId, out var team))
            {
                if (globalMembers.Contains(memberName))
                {
                    BugsBunny.Log($"Member {memberName} is already part of a team.",this);
                    return false;
                }

                if (team.AddMember(memberName))
                {
                    globalMembers.Add(memberName); // Add to global member tracker
                    BugsBunny.Log($"Member {memberName} added to team {team.TeamName}.",this);
                    return true;
                }
                BugsBunny.Log($"Member {memberName} is already in team {team.TeamName}.",this);
                return false;
            }

            BugsBunny.Log($"Team {teamId} is not registered.",this);
            return false;
        }
        public  bool RemoveMemberFromTeam(GlobalEnums.GameTeam teamId, string memberName)
        {
            if (teams.TryGetValue(teamId, out var team))
            {
                if (team.RemoveMember(memberName))
                {
                    globalMembers.Remove(memberName); // Remove from global member tracker
                    BugsBunny.Log($"Member {memberName} removed from team {team.TeamName}.",this);
                    return true;
                }

                BugsBunny.LogYellow($"Member {memberName} is not in team {team.TeamName}.",this);
                return false;
            }

            BugsBunny.LogYellow($"Team {teamId} is not registered.",this);
            return false;
        }
        public  TeamData GetTeamData(GlobalEnums.GameTeam teamId)
        {
            if (teams.TryGetValue(teamId, out var team))
            {
                return team;
            }
            BugsBunny.LogYellow($"Team {teamId} is not registered.",this);
            return null;
        }
        public  Dictionary<GlobalEnums.GameTeam, TeamData> GetAllTeamData()
        {
            return teams;
        }
        public  Dictionary<GlobalEnums.GameTeam, Color> GetAllTeamIds()
        {
            var teamColors = new Dictionary<GlobalEnums.GameTeam, Color>();

            foreach (var team in teams)
            {
                teamColors[team.Key] = team.Value.TeamColor;
            }

            return teamColors;
        }
        public  void ClearAllTeams()
        {
            teams.Clear();
            globalMembers.Clear();
            BugsBunny.Log("All teams and members cleared.",this);
        }

        public bool LogThisClass { get; } = true;
    } //TeamManager
} //RedGaint