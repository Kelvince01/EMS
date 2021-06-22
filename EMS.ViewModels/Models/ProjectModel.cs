#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;
using System.Collections.Generic;
using EMS.Data.Data;
using EMS.ViewModels.Infrastructure.ViewModels;
using EMS.ViewModels.Services;

namespace EMS.ViewModels.Models
{
    public class ProjectModel : ObservableObject
    {
        static public ProjectModel CreateEmpty() => new ProjectModel { ProjectID = -1, CustomerID = -1, EmployeeID = -1, IsEmpty = true };

        public long ProjectID { get; set; }

        public int CategoryID { get; set; }

        public long CustomerID { get; set; }

        public long EmployeeID { get; set; }

        public virtual CustomerModel Customer { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

        public int Progress { get; set; }
        private int _status;
        public int Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        public decimal Price { get; set; }
        public int TaxType { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModifiedOn { get; set; }

        public byte[] Picture { get; set; }
        public object PictureSource { get; set; }

        public byte[] Thumbnail { get; set; }
        public object ThumbnailSource { get; set; }

        public bool IsNew => ProjectID <= 0;
        public string CategoryName => LookupTablesProxy.Instance.GetCategory(CategoryID);

        public string StatusDesc => LookupTablesProxy.Instance.GetOrderStatus(Status);

        /*private void UpdateStatusDependencies()
        {
            switch (Status)
            {
                case 0:
                case 1:
                    StartDate = null;
                    EndDate = null;
                    break;
                case 2:
                    StartDate = StartDate ?? CreatedOn;
                    EndDate = null;
                    break;
                case 3:
                    StartDate = StartDate ?? CreatedOn;
                    EndDate = EndDate ?? StartDate ?? CreatedOn;
                    break;
            }

            NotifyPropertyChanged(nameof(StatusDesc));
        }*/

        public override void Merge(ObservableObject source)
        {
            if (source is ProjectModel model)
            {
                Merge(model);
            }
        }

        public void Merge(ProjectModel source)
        {
            if (source != null)
            {
                ProjectID = source.ProjectID;
                CategoryID = source.CategoryID;
                EmployeeID = source.EmployeeID;
                CustomerID = source.CustomerID;
                Name = source.Name;
                Description = source.Description;
                Price = source.Price;
                TaxType = source.TaxType;
                CreatedOn = source.CreatedOn;
                LastModifiedOn = source.LastModifiedOn;
                Status = source.Status;
                Progress = source.Progress;
                Picture = source.Picture;
                PictureSource = source.PictureSource;
                Thumbnail = source.Thumbnail;
                ThumbnailSource = source.ThumbnailSource;
            }
        }

        public override string ToString()
        {
            return ProjectID.ToString();
        }
    }
}
