//by fady
const hamMenu = document.querySelector(".ham-menu");
const offScreenMenu = document.querySelector(".off-screen-menu");

hamMenu.addEventListener("click", () => {
    hamMenu.classList.toggle("active");
    offScreenMenu.classList.toggle("active");
});


function updateQuantity(element, delta) {
    const quantitySpan = element.parentElement.querySelector('.quantity');
    let quantity = parseInt(quantitySpan.textContent);
    quantity += delta;
    if (quantity < 1) quantity = 1;
    quantitySpan.textContent = quantity;
}



// the filter price and range in the shop page:

const slider = document.querySelector(".range-slider");
const progress = slider.querySelector(".progress");
const minPriceInput = slider.querySelector(".min-price");
const maxPriceInput = slider.querySelector(".max-price");
const minInput = slider.querySelector(".min-input");
const maxInput = slider.querySelector(".max-input");

const updateProgress = () => {
  const minValue = parseInt(minInput.value);
  const maxValue = parseInt(maxInput.value);

  // get the total range of the slider
  const range = maxInput.max - minInput.min;
  // get the selected value range of the progress
  const valueRange = maxValue - minValue;
  // calculate the width percentage
  const width = (valueRange / range) * 100;
  // calculate the min thumb offset
  const minOffset = ((minValue - minInput.min) / range) * 100;

  // update the progress width
  progress.style.width = width + "%";
  // update the progress left position
  progress.style.left = minOffset + "%";

  // update the number inputs
  minPriceInput.value = minValue;
  maxPriceInput.value = maxValue;
};

const updateRange = (event) => {
  const input = event.target;

  let min = parseInt(minPriceInput.value);
  let max = parseInt(maxPriceInput.value);

  if (input === minPriceInput && min > max) {
    max = min;
    maxPriceInput.value = max;
  } else if (input === maxPriceInput && max < min) {
    min = max;
    minPriceInput.value = min;
  }

  minInput.value = min;
  maxInput.value = max;

  updateProgress();
};

minPriceInput.addEventListener("input", updateRange);
maxPriceInput.addEventListener("input", updateRange);

minInput.addEventListener("input", () => {
  if (parseInt(minInput.value) >= parseInt(maxInput.value)) {
    maxInput.value = minInput.value;
  }
  updateProgress();
});

maxInput.addEventListener("input", () => {
  if (parseInt(maxInput.value) <= parseInt(minInput.value)) {
    minInput.value = maxInput.value;
  }
  updateProgress();
});

let isDragging = false;
let startOffsetX;

progress.addEventListener("mousedown", (e) => {
  e.preventDefault(); // prevent text selection

  isDragging = true;

  startOffsetX = e.clientX - progress.getBoundingClientRect().left;

  slider.classList.toggle("dragging", isDragging);
});

document.addEventListener("mousemove", (e) => {
  if (isDragging) {
    // get the size and position of the slider
    const sliderRect = slider.getBoundingClientRect();
    const progressWidth = parseFloat(progress.style.width || 0);

    // calculate the new left position for the progress element
    let newLeft =
      ((e.clientX - sliderRect.left - startOffsetX) / sliderRect.width) * 100;

    // ensure the progress is not exceeding the slider boundaries
    newLeft = Math.min(Math.max(newLeft, 0), 100 - progressWidth);

    // update the progress position
    progress.style.left = newLeft + "%";

    // calculate the new min thumb position
    const range = maxInput.max - minInput.min;
    const newMin = Math.round((newLeft / 100) * range) + parseInt(minInput.min);
    const newMax = newMin + parseInt(maxInput.value) - parseInt(minInput.value);

    // update the min input
    minInput.value = newMin;
    maxInput.value = newMax;

    // update the progress
    updateProgress();
  }
  slider.classList.toggle("dragging", isDragging);
});

document.addEventListener("mouseup", () => {
  if (isDragging) {
    isDragging = false;
  }
  slider.classList.toggle("dragging", isDragging);
});

updateProgress();


function addToCart(name, price) {
    // Placeholder for cart functionality
    alert(`Added ${name} to cart for $${price}`);
    // Implement actual cart logic here, e.g., using session storage or API calls
}

// Handle active navigation links
document.addEventListener('DOMContentLoaded', function() {
    // Get the current URL path
    const currentPath = window.location.pathname;
    
    // Get all navigation links in both regular and mobile menus
    const navLinks = document.querySelectorAll('.nav-links a, .off-screen-menu a');
    
    // Check each link to see if it matches the current path
    navLinks.forEach(link => {
        const linkPath = link.getAttribute('href');
        
        // Skip if it's a dropdown toggle
        if (link.classList.contains('dropdown-toggle')) return;
        
        // If the link path matches the current path or is a substring of it
        if (linkPath && (currentPath === linkPath || currentPath.startsWith(linkPath) && linkPath !== '/')) {
            link.classList.add('active');
        }
    });
    
    // Monitor window resize for responsive adaptations
    function handleWindowResize() {
        const windowWidth = window.innerWidth;
        
        // Adjust UI based on window size
        if (windowWidth < 768) {
            // Mobile adjustments
            document.querySelectorAll('.dashboard-card').forEach(card => {
                card.classList.add('mb-3');
            });
        } else {
            // Desktop adjustments
            document.querySelectorAll('.dashboard-card').forEach(card => {
                card.classList.remove('mb-3');
            });
        }
    }
    
    // Initial call to set correct state
    handleWindowResize();
    
    // Add resize event listener
    window.addEventListener('resize', handleWindowResize);
    
    // Close mobile menu when clicking outside
    document.addEventListener('click', function(event) {
        const offScreenMenu = document.querySelector('.off-screen-menu');
        const hamMenu = document.querySelector('.ham-menu');
        
        if (offScreenMenu && offScreenMenu.classList.contains('active') && 
            !offScreenMenu.contains(event.target) && 
            !hamMenu.contains(event.target)) {
            hamMenu.classList.remove('active');
            offScreenMenu.classList.remove('active');
        }
    });
});

// Update the account page specific functionality to fix card display issues
(function() {
    document.addEventListener('DOMContentLoaded', function() {
        // Check if we're on an account page
        const isAccountPage = window.location.pathname.includes('/Account/') || 
                             window.location.pathname.includes('/Address/') ||
                             window.location.pathname.includes('/Order/') ||
                             window.location.pathname.includes('/Wishlist/');
        
        if (isAccountPage) {
            // Fix dashboard cards layout
            const dashboardCards = document.querySelectorAll('.dashboard-card');
            if (dashboardCards.length > 0) {
                // Set a timeout to ensure DOM is fully loaded
                setTimeout(() => {
                    // Get the container width
                    const contentWrapper = document.querySelector('.main-content-wrapper');
                    if (contentWrapper) {
                        const containerWidth = contentWrapper.offsetWidth;
                        const cardWidth = Math.floor((containerWidth - 40) / 3); // 40px for padding
                        
                        // Apply explicit width for card consistency if in desktop view
                        if (window.innerWidth >= 992) {
                            dashboardCards.forEach(card => {
                                // Clear any inline styles first to prevent conflicts
                                card.style.width = '';
                                card.style.maxWidth = '';
                                
                                // Set proper dimensions
                                card.style.minHeight = '180px';
                            });
                        } else if (window.innerWidth >= 576 && window.innerWidth < 992) {
                            // For tablet view - 2 cards per row
                            dashboardCards.forEach(card => {
                                card.style.minHeight = '160px';
                            });
                        } else {
                            // For mobile view - 1 card per row
                            dashboardCards.forEach(card => {
                                card.style.minHeight = '140px';
                            });
                        }
                    }
                }, 100);
            }
            
            // Fix address notification positioning on multi-window display
            const addressNotification = document.querySelector('.address-notification');
            if (addressNotification) {
                // Check if we're in a multi-window setup by evaluating window width
                // compared to screen width (significantly different in multi-window)
                if (window.innerWidth < window.screen.width * 0.8) {
                    addressNotification.classList.add('position-relative');
                }
                
                // Add nice hover effect
                addressNotification.addEventListener('mouseenter', function() {
                    this.style.transform = 'translateY(-3px)';
                    this.style.boxShadow = '0 6px 12px rgba(0,0,0,0.1)';
                    this.style.transition = 'all 0.3s ease';
                });
                
                addressNotification.addEventListener('mouseleave', function() {
                    this.style.transform = 'translateY(0)';
                    this.style.boxShadow = '0 2px 4px rgba(0,0,0,0.1)';
                });
            }
            
            // Fix main content wrapper positioning
            const mainContentWrapper = document.querySelector('.main-content-wrapper');
            if (mainContentWrapper) {
                if (window.innerWidth < 768) {
                    mainContentWrapper.style.margin = '0 auto';
                    mainContentWrapper.style.maxWidth = '100%';
                }
            }
            
            // Handle resize for responsive layout
            window.addEventListener('resize', function() {
                // Recalculate dashboard card dimensions
                if (dashboardCards.length > 0) {
                    if (window.innerWidth >= 992) {
                        dashboardCards.forEach(card => {
                            card.style.minHeight = '180px';
                        });
                    } else if (window.innerWidth >= 576 && window.innerWidth < 992) {
                        dashboardCards.forEach(card => {
                            card.style.minHeight = '160px';
                        });
                    } else {
                        dashboardCards.forEach(card => {
                            card.style.minHeight = '140px';
                        });
                    }
                }
                
                // Recalculate layout dimensions
                if (mainContentWrapper) {
                    if (window.innerWidth < 768) {
                        mainContentWrapper.style.margin = '0 auto';
                        mainContentWrapper.style.maxWidth = '100%';
                    } else {
                        mainContentWrapper.style.margin = '';
                        mainContentWrapper.style.maxWidth = '';
                    }
                }
                
                // Handle address notification responsive positioning
                if (addressNotification) {
                    if (window.innerWidth < window.screen.width * 0.8) {
                        addressNotification.classList.add('position-relative');
                    } else {
                        addressNotification.classList.remove('position-relative');
                    }
                }
            });
        }
    });
})();

// Add smooth account layout management for the redesigned interface
document.addEventListener('DOMContentLoaded', function() {
    // Check if we're on an account page
    const isAccountPage = window.location.pathname.includes('/Account/') || 
                         window.location.pathname.includes('/Address/') ||
                         window.location.pathname.includes('/Order/') ||
                         window.location.pathname.includes('/Wishlist/');
    
    if (isAccountPage) {
        // Stat cards hover effects
        const statCards = document.querySelectorAll('.stat-card');
        if (statCards.length > 0) {
            statCards.forEach(card => {
                card.addEventListener('mouseenter', function() {
                    const icon = this.querySelector('.stat-card-icon');
                    if (icon) {
                        icon.style.transform = 'scale(1.1) rotate(5deg)';
                    }
                });
                
                card.addEventListener('mouseleave', function() {
                    const icon = this.querySelector('.stat-card-icon');
                    if (icon) {
                        icon.style.transform = '';
                    }
                });
            });
        }
        
        // Add fade-in animation to all section cards
        const sectionCards = document.querySelectorAll('.section-card');
        if (sectionCards.length > 0) {
            sectionCards.forEach((card, index) => {
                card.style.opacity = '0';
                card.style.transform = 'translateY(20px)';
                card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
                
                setTimeout(() => {
                    card.style.opacity = '1';
                    card.style.transform = 'translateY(0)';
                }, 100 + (index * 100));
            });
        }
        
        // Mobile menu toggle with smooth animation
        const toggleBtn = document.getElementById('toggleAccountMenu');
        const mobileMenu = document.getElementById('mobileAccountMenu');
        
        if (toggleBtn && mobileMenu) {
            toggleBtn.addEventListener('click', function() {
                this.classList.toggle('active');
                mobileMenu.classList.toggle('show');
                
                // If menu is now visible, add active animation to each nav item
                if (mobileMenu.classList.contains('show')) {
                    const menuItems = mobileMenu.querySelectorAll('.nav-item');
                    menuItems.forEach((item, index) => {
                        item.style.opacity = '0';
                        item.style.transform = 'translateX(-20px)';
                        item.style.transition = 'opacity 0.3s ease, transform 0.3s ease';
                        
                        setTimeout(() => {
                            item.style.opacity = '1';
                            item.style.transform = 'translateX(0)';
                        }, 50 + (index * 50));
                    });
                }
            });
        }
        
        // Status banner hover effect
        const statusBanner = document.querySelector('.status-banner');
        if (statusBanner) {
            statusBanner.addEventListener('mouseenter', function() {
                const icon = this.querySelector('.status-banner-icon');
                if (icon) {
                    icon.style.transform = 'scale(1.1)';
                    icon.style.transition = 'transform 0.3s ease';
                }
            });
            
            statusBanner.addEventListener('mouseleave', function() {
                const icon = this.querySelector('.status-banner-icon');
                if (icon) {
                    icon.style.transform = '';
                }
            });
        }
        
        // Responsive adjustments
        function handleResize() {
            const mainContent = document.querySelector('.main-content');
            const accountContainer = document.querySelector('.account-container');
            
            if (window.innerWidth < 768) {
                if (mainContent) {
                    mainContent.style.marginTop = '20px';
                }
                if (accountContainer) {
                    accountContainer.style.padding = '15px';
                }
            } else {
                if (mainContent) {
                    mainContent.style.marginTop = '0';
                }
                if (accountContainer) {
                    accountContainer.style.padding = '';
                }
            }
        }
        
        // Initial call and event listener
        handleResize();
        window.addEventListener('resize', handleResize);
    }
});
