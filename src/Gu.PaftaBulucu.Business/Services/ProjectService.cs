using System;
using Gu.PaftaBulucu.Business.Dtos;
using Gu.PaftaBulucu.Data.Models;
using Gu.PaftaBulucu.Data.Repositories;
using Nelibur.ObjectMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gu.PaftaBulucu.Business.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IDatabaseRepository<Project> _projectRepository;

        public ProjectService(IDatabaseRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<ProjectDto>> GetProjects(string email)
        {
            TinyMapper.Bind<List<Project>, List<ProjectDto>>();
            var projects = await _projectRepository.FindAsync(p => p.Email == email);
            return TinyMapper.Map<List<ProjectDto>>(projects);
        }

        public async Task<ProjectDto> GetProject(int projectId)
        {
            TinyMapper.Bind<Project, ProjectDto>();
            var project = await _projectRepository.GetByIdAsync(projectId);
            return TinyMapper.Map<ProjectDto>(project);
        }

        public async Task<ProjectDto> AddProject(string email, SaveProjectDto projectDto)
        {
            TinyMapper.Bind<SaveProjectDto, Project>();
            TinyMapper.Bind<Project, ProjectDto>();
            var project = TinyMapper.Map<Project>(projectDto);

            project.Created = UnixTimeStamp();
            project.Email = email;

            await _projectRepository.AddAsync(project);

            await _projectRepository.CommitAsync();

            return TinyMapper.Map<ProjectDto>(project);
        }

        public async Task UpdateProject(ProjectDto projectDto)
        {
            TinyMapper.Bind<SaveProjectDto, Project>();
            var project = TinyMapper.Map<Project>(projectDto);

            _projectRepository.NotifyChange(project);

            await _projectRepository.CommitAsync();
        }

        public async Task DeleteProject(int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
                return;

            _projectRepository.Remove(project);

            await _projectRepository.CommitAsync();
        }

        private int UnixTimeStamp()
        {
            return (int)DateTime.UtcNow.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        /*
        TODO: DELETE AFTER MIGRATION

        private readonly Dictionary<int, int> _scaleRanges = new Dictionary<int, int>
        {
            {100, 18000000},
            {50, 9000000},
            {25, 4500000},
            {10, 1800000},
            {5, 900000},
            {2, 450000},
            {1, 225000}

        };

        public async Task MigrateProjects()
        {
            var projects = await _projectRepository.GetAllAsync();
            var sheetNameRegex =
                new Regex(@"^([a-zA-ZsçÇöÖşŞıİğĞüÜ]*?\-?[A-Z]\s[0-9]{2})\-?([abcd])?([1-4])?\-?([0-9]{1,2})?\-?([abcd])?\-?([1-4])?\-?([abcd])?");
            
            foreach (var project in projects)
            {
                foreach (var sheetEntry in project.Entries)
                {
                    var name = sheetEntry.Name.Split('-').Select(s => s.Trim());
                    sheetEntry.Name = string.Join("-", name);
                    var matchedSheetParts = sheetNameRegex.Match(sheetEntry.Name);
                    if (matchedSheetParts.Success)
                    {
                        var matchCount = matchedSheetParts.Groups.Values.Count(v => v.Value.Length > 0);
                        matchCount += matchCount < 4 || matchCount == 4 && matchedSheetParts.Groups[3].Value.Length > 0 ? -2 : -1;
                        var newScale = _scaleRanges.Keys.ToList()[matchCount];
                        if (sheetEntry.Scale != newScale)
                        {
                            sheetEntry.Scale = newScale;
                            _projectRepository.NotifyChange(project);
                        }
                    }


                    if (sheetEntry.Lat > 180)
                    {
                        sheetEntry.Lat /= 36000000;
                    }

                    if (sheetEntry.Lng > 180)
                    {
                        sheetEntry.Lng /= 36000000;
                    }
                }
               
                await _projectRepository.CommitAsync();
            }
        }
        */
    }
}
